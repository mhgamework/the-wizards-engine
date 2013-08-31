

namespace MHGameWork.TheWizards.Rendering.Vegetation.Grass
{
    public interface IGrassMap
    {
        int Width { get; }
        int Length { get; }

        /*
         * value representing the height of the land y-coordinaat
         */
        float getHeight(int posx, int posy);
        /*
         *  The amount of grassmeshes added to one square If set to zero no grass is made at all
         */
        int getDensity(int posx, int posy);
        /*
         * value between 0 and 1 representing the maximum growing height
         */
        float getGrowingHeight(int posx,int posy);
    }
}