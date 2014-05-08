using System;
using System.Linq;
using System.Runtime;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Particles;
using MHGameWork.TheWizards.Rendering.Particles;
using MHGameWork.TheWizards.Scattered.GameLogic.SpellCasting;
using MHGameWork.TheWizards.Testing;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered._Tests.GameLogic
{
    [EngineTest]
    [TestFixture]
    public class SpellCastingEffectsTest
    {
        private readonly IRenderingTester r;
        private readonly GeometrySampler sampler;

        public SpellCastingEffectsTest(IRenderingTester r, GeometrySampler sampler)
        {
            this.r = r;
            this.sampler = sampler;
        }

        public void TestBurstEffect()
        {
            r.SetCameraPosition(new Vector3(0, 1, 2), new Vector3(0, 0, -3));

            var effect = new BurstSpellEffect(r.CreateParticleEffect());

            loopEffect(effect.Start, effect.Stop);
        }



        public void TestBeamEffect()
        {
            r.SetCameraPosition(new Vector3(0, 1, 2), new Vector3(0, 0, -3));

            var effect = new BeamSpellEffect(r.CreateParticleEffect(), sampler);

            loopEffect(effect.Start, effect.Stop);


        }

        public void TestImpactEffect()
        {

            r.SetCameraPosition(new Vector3(0, 0, 5), new Vector3(0, 0, 0));

            var effect = new ImpactSpellEffect(r.CreateParticleEffect(), sampler);

            loopEffect(effect.Start, effect.Stop);

        }

        public void TestShieldEffect()
        {

            r.SetCameraPosition(new Vector3(0, 0, 5), new Vector3(0, 0, 0));

            var playbackSpeed = 0.3f;

            var effect = r.CreateParticleEffect();

            var emitter = effect.CreateCustomEmitter(1 / 300f / playbackSpeed, p =>
                {
                    p.StartPosition = new Vector3(sampler.RandomPointOnCircle(), 0) * 0.1f;
                    p.StartVelocity = new Vector3(sampler.RandomPointOnCircle(), 0);
                    p.Size = 0.05f;
                    p.Duration = 1 / 3f / playbackSpeed;
                });

            effect.CalculatePosition = p =>
                {
                    var lifeTime = (effect.CurrentTime - p.SpawnTime) * 3 * playbackSpeed;

                    var magicNumber = 0.4f;

                    var startAngle = p.StartVelocity.X * 1000;
                    var endAngle = p.StartVelocity.Y * 1000 % magicNumber + (1 - magicNumber);

                    var ret = p.StartPosition;

                    ret.X += (float)Math.Pow(lifeTime, 0.8f) * (float)Math.Cos(Math.Sqrt(lifeTime * 10 * endAngle) + startAngle);
                    ret.Y += (float)Math.Pow(lifeTime, 0.8f) * (float)Math.Sin(Math.Sqrt(lifeTime * 10 * endAngle) + startAngle);
                    ret.Z += ret.TakeXY().Length() * 0.5f;

                    return ret;
                };



            loopEffect(() => { emitter.Start(); }, () =>
                {
                    emitter.Stop();
                });

        }

        public void TestMineEffect()
        {

            r.SetCameraPosition(new Vector3(0, 3, 5), new Vector3(0, 0, 0));

            var playbackSpeed = 0.3f;

            var effect = r.CreateParticleEffect();

            var emitter = effect.CreateCustomEmitter(1 / 10f / playbackSpeed, p =>
                {
                    p.StartPosition = sampler.RandomPointOnCircleSurface().ToXZ(0.2f);
                    p.StartVelocity = new Vector3(0, -0.2f, 0);
                    p.Size = 0.05f;
                    p.Duration = 1 / 3f / playbackSpeed;
                });

            /*effect.CalculatePosition = p =>
            {
                var lifeTime = (effect.CurrentTime - p.SpawnTime) * 3 * playbackSpeed;

                var magicNumber = 0.4f;

                var startAngle = p.StartVelocity.X * 1000;
                var endAngle = p.StartVelocity.Y * 1000 % magicNumber + (1 - magicNumber);

                var ret = p.StartPosition;

                ret.X += (float)Math.Pow(lifeTime, 0.8f) * (float)Math.Cos(Math.Sqrt(lifeTime * 10 * endAngle) + startAngle);
                ret.Y += (float)Math.Pow(lifeTime, 0.8f) * (float)Math.Sin(Math.Sqrt(lifeTime * 10 * endAngle) + startAngle);
                ret.Z += ret.TakeXY().Length() * 0.5f;

                return ret;
            };*/



            loopEffect(() => { emitter.Start(); }, () =>
            {
                emitter.Stop();
            });

        }

        public void TestMissileEffect()
        {

            var target = new Vector3(0, 0, -10);

            var currentPos = new Vector3(0, 3, 5);
            var currentVelocity = new Vector3(0.2f, 1, -1);

            r.SetCameraPosition(new Vector3(0, 3, 5), new Vector3(0, 0, -10));

            var playbackSpeed = 0.3f;

            var effect = r.CreateParticleEffect();

            var emitter = effect.CreateCustomEmitter(1 / 10f / playbackSpeed, p =>
                {
                    p.StartPosition = currentPos;
                    p.StartVelocity = currentVelocity;
                    p.Size = 0.05f;
                    p.Duration = 1 / 3f / playbackSpeed;
                });

            /*effect.CalculatePosition = p =>
            {
                var lifeTime = (effect.CurrentTime - p.SpawnTime) * 3 * playbackSpeed;

                var magicNumber = 0.4f;

                var startAngle = p.StartVelocity.X * 1000;
                var endAngle = p.StartVelocity.Y * 1000 % magicNumber + (1 - magicNumber);

                var ret = p.StartPosition;

                ret.X += (float)Math.Pow(lifeTime, 0.8f) * (float)Math.Cos(Math.Sqrt(lifeTime * 10 * endAngle) + startAngle);
                ret.Y += (float)Math.Pow(lifeTime, 0.8f) * (float)Math.Sin(Math.Sqrt(lifeTime * 10 * endAngle) + startAngle);
                ret.Z += ret.TakeXY().Length() * 0.5f;

                return ret;
            };*/


            r.ObserveUpdate(elapsed =>
                {
                    currentVelocity += Vector3.Normalize(target - currentPos);
                    // TODO: cap velocity?
                    currentPos += currentVelocity * elapsed;
                    r.LineManager3D.AddCenteredBox(currentPos, 0.2f, new Color4(1, 0, 0));
                });


            loopEffect(() => { emitter.Start(); }, () =>
            {
                emitter.Stop();
            });

        }

        public void TestEarthRuptureEffect()
        {
            r.SetCameraPosition(new Vector3(0, 3, 5), new Vector3(0, 0, -10));

            var running = false;

            var numRuptures = 5;
            var ruptureInterval = 0.5f;
            var ruptureDistance = 2;
            var impacts =
                Enumerable.Range(0, numRuptures).Select(_ => new ImpactSpellEffect(r.CreateParticleEffect(), sampler)).ToArray();


            var ruptureCount = 0;

            Action rupture = null;
            rupture = () =>
                {
                    if (!running) return;
                    var pos = new Vector3(0, 0, (-ruptureCount + 1) * ruptureDistance);
                    var imp = impacts[ruptureCount];
                    imp.Position = pos; // Maybe move position to particle effect?
                    imp.Start();

                    ruptureCount++;
                    r.SetTimeout(ruptureInterval, rupture);

                };

            loopEffect(() =>
            {
                running = true;
                rupture();
            }, () =>
                {
                    running = false;
                    impacts.ForEach(i => i.Stop());
                });

        }

        public void TestEruption()
        {
            r.SetCameraPosition(new Vector3(5, 5, 5), new Vector3(0, 0, 0));

            var effect = r.CreateParticleEffect();

            loopEffect(() =>
            {
                for (int i = 0; i < 50; i++)
                {
                    var p = new ColoredParticle();
                    p.StartPosition = new Vector3(0, 0, 0);
                    p.Color = new Color4(1, 0, 0);
                    p.Size = 0.1f;
                    p.StartVelocity = (sampler.RandomPointOnSphere()*6);

                    p.Duration = 1f;
                    p.SpawnTime = effect.CurrentTime;


                    effect.AddParticle(p);
                }
            }, () =>
            {
            });

        }

        private void loopEffect(Action startAction, Action endAction)
        {

            r.SetRepeat(3, () =>
            {
                startAction();
                r.SetTimeout(2, endAction);
            });
        }
    }
}