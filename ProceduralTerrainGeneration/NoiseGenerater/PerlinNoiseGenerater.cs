using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards;
using Microsoft.Xna.Framework;

namespace TreeGenerator.NoiseGenerater
{
    public class PerlinNoiseGenerater
    {
        public Vector2 Offset { get; set; }
        private Seeder seeder = new Seeder(79465);
        public void CreateRandomOffset()
        {
            Offset = seeder.NextVector2(-Vector2.One*1000, Vector2.One*1000);
        }

        public float persistance;
        public int NumberOfOctaves;
        float  randomNumberGenerater(int x,int y)
        {
            int n = x + y*57;
            n = (n<<13)^n;

            return (float)( 1.0 - ( (n*(n * n * 15731 + 789221) + 1376312589)) / 1073741824.0);
        }
        float CosineInterpolation(float a, float b, float x)
        {
            float ft = x*MathHelper.Pi;
            float f = (float)(1 - Math.Cos((float) ft))*0.5f;

            return a * (1 - f) + b * f;
        }
        
        float smoothNoise2D(int x, int y)
        {
            float corners = (randomNumberGenerater(x - 1, y - 1) + randomNumberGenerater(x + 1, y - 1) + randomNumberGenerater(x - 1, y + 1) + randomNumberGenerater(x + 1, y + 1))/16;
            float sides = (randomNumberGenerater(x - 1, y) + randomNumberGenerater(x + 1, y) + randomNumberGenerater(x, y - 1) + randomNumberGenerater(x, y + 1)) / 8;
            float center = randomNumberGenerater(x, y)/4;
          
            return (corners + sides + center)*0.5f;
        }
       public  float interpolatedNoise(float x, float y)
        {
            int wholeX = (int)x;
            float fractionalX = x - wholeX;
            int wholeY = (int)y;
            float fractionalY = y - wholeY;

            float n1 = smoothNoise2D(wholeX, wholeY);
            float n2 = smoothNoise2D(wholeX+1, wholeY);
            float n3 = smoothNoise2D(wholeX , wholeY+1);
            float n4 = smoothNoise2D(wholeX + 1, wholeY+1);

            float interPolated1=CosineInterpolation(n1, n2, fractionalX);
            float interPolated2 = CosineInterpolation(n3, n4, fractionalX);

            return CosineInterpolation(interPolated1, interPolated2, fractionalY);

        }

        public float GetPerlineNoise(float x,float y)
        {
            float total = 0;
            for (int i = 0; i <NumberOfOctaves ; i++)
            {
                int frequency = 2^i;
                float amplitude = (float) Math.Pow((double) persistance, (double) i);
                total = total + interpolatedNoise(x*frequency, y*frequency)*amplitude;
            }
            return total;
        }
        public float GetPerlineNoise(float x, float y,int octaves,float frequency,float lacunrity,float gain)
        {
            float total = 0;
            float amp = 1.0f;
            float ampTotal = 0;
            for (int i = 0; i < octaves; i++)
            {
            
                total = total + interpolatedNoise(x * frequency, y * frequency) * amp;
                ampTotal += amp;
                amp *= gain;
                frequency *= lacunrity;
            }
            return total/ampTotal;
        }
       
        public float GetFractalBrowningNoise(float x,float y,int octaves,float lacunarity,float _factorX,float _factorY)
        {
            float noise = 0;
            float frequency = 0.03f;
            float amplitude = 4f;
            
            for (int i = 0; i < octaves; i++)
            {
                //persistance = _persistance*(2^i);
                //NumberOfOctaves = i;

                noise += interpolatedNoise(x * frequency, y * frequency) * amplitude;
                frequency *= lacunarity;
                amplitude *= _factorX;
            }
            return noise;
        }
        public float RidgedMF(float x,float y,float startfrequency, int octaves,float lacunarity,float gain,float offset)
        {
            float noise = 0;
            float amplitude = 10f;
            float frequency =startfrequency;
            float prev = 1.0f;
            for (int i = 0; i < octaves; i++)
            {
                float n = ridge(interpolatedNoise(x*frequency, y*frequency), offset);
                noise += n * amplitude * prev;
                prev = n;
                frequency *= lacunarity;
                amplitude *= gain;
            }
            return noise;
        }
        private float ridge(float h,float offset)
        {
            h = Math.Abs(h);
            h = offset - h;
            h = h * h;

            return h;
        }

     
        public Vector2 Perturb(float x,float y ,float frequency, float distance)
        {
            float Size;
            Vector2 vec;
         
            //vec=new Vector2( x + (interpolatedNoise(frequency * x , frequency * y ) * distance),y + (interpolatedNoise(frequency *y, frequency * x) * distance));
            vec = new Vector2((x + (GetPerlineNoise(x, y, 4, 0.1f , 3.0f , 0.5f) - 0.5f) * 2.0f * distance), y + ((GetPerlineNoise(x, y, 4, 0.1f , 2.0f , 0.5f) - 0.5f) * 2.0f * distance));
            return vec;
        }

        public float CombinedFractalBrowningAndRidgedMF(float x,float y,int octaves,float lacunarityFractal,float lacunarityRidge,float gainFractal,float gainRidge,float offset,float distribution )
        {
            x *= (1/5f);
            y *= (1 / 5f);
            float ridgeMF = RidgedMF(x, y,0.08f, octaves, lacunarityRidge, gainRidge, offset)*distribution;
            float fractalBrowning = GetFractalBrowningNoise(x, y, octaves, lacunarityFractal, gainFractal, gainFractal)*(1-distribution);
            return ridgeMF + fractalBrowning;
        }
    }
}
