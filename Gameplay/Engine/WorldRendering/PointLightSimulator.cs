namespace MHGameWork.TheWizards.Engine.WorldRendering
{
    /// <summary>
    /// Responsible for rendering PointLight's
    /// </summary>
    public class PointLightSimulator : ISimulator
    {
        public class Data : IModelObjectAddon<PointLight>
        {
            private Rendering.Deferred.PointLight el;

            public void Update(PointLight light)
            {
                if (light.Enabled && el == null)
                    el = TW.Graphics.AcquireRenderer().CreatePointLight();
                if (!light.Enabled && el != null)
                {
                    //Delete not supported :p
                }
                updateElement(light);
            }

            private void updateElement(PointLight light)
            {
                if (el == null) return;
                el.LightPosition = light.Position;
                el.LightRadius = light.Size;
                el.LightIntensity = light.Intensity;
                el.ShadowsEnabled = light.ShadowsEnabled;
            }


            public void Dispose()
            {
            }
        }

        public void Simulate()
        {
            TW.Data.EnsureAttachment<PointLight, Data>(o => new Data());
            foreach (var light in TW.Data.GetChangedObjects<PointLight>())
            {
                light.get<Data>().Update(light);
            }
        }
    }
}
