using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuclex {

  internal class GraphicsDeviceInformationComparer : IComparer<GraphicsDeviceInformation> {
    // Fields
    private GraphicsDeviceManager graphics;

    // Methods
    public GraphicsDeviceInformationComparer(GraphicsDeviceManager graphicsComponent) {
      this.graphics = graphicsComponent;
    }

    public int Compare(GraphicsDeviceInformation d1, GraphicsDeviceInformation d2) {
      float num5;
      if(d1.DeviceType != d2.DeviceType) {
        if(d1.DeviceType >= d2.DeviceType) {
          return 1;
        }
        return -1;
      }

        
      // MHGW

      /*CreateOptions options = CreateOptions.MixedVertexProcessing | CreateOptions.HardwareVertexProcessing | CreateOptions.SoftwareVertexProcessing;
      CreateOptions options2 = d1.CreationOptions & options;
      CreateOptions options3 = d2.CreationOptions & options;
      if(options2 != options3) {
        if(options2 == CreateOptions.HardwareVertexProcessing) {
          return -1;
        }
        if(options3 != CreateOptions.HardwareVertexProcessing) {
          if(options2 == CreateOptions.MixedVertexProcessing) {
            return -1;
          }
          if(options3 == CreateOptions.MixedVertexProcessing) {
            return 1;
          }
          if(options2 != CreateOptions.None) {
            return -1;
          }
        }
        return 1;
      }*/
      PresentationParameters presentationParameters = d1.PresentationParameters;
      PresentationParameters parameters2 = d2.PresentationParameters;
      if(presentationParameters.IsFullScreen != parameters2.IsFullScreen) {
        if(this.graphics.IsFullScreen != presentationParameters.IsFullScreen) {
          return 1;
        }
        return -1;
      }
      int num = this.RankFormat(presentationParameters.BackBufferFormat);
      int num2 = this.RankFormat(parameters2.BackBufferFormat);
      if(num != num2) {
        if(num >= num2) {
          return 1;
        }
        return -1;
      }
      if(presentationParameters.MultiSampleType != parameters2.MultiSampleType) {
        int num3 = (presentationParameters.MultiSampleType == MultiSampleType.NonMaskable) ? ((int)(MultiSampleType.SixteenSamples | MultiSampleType.NonMaskable)) : ((int)presentationParameters.MultiSampleType);
        int num4 = (parameters2.MultiSampleType == MultiSampleType.NonMaskable) ? ((int)(MultiSampleType.SixteenSamples | MultiSampleType.NonMaskable)) : ((int)parameters2.MultiSampleType);
        if(num3 <= num4) {
          return 1;
        }
        return -1;
      }
      if(presentationParameters.MultiSampleQuality != parameters2.MultiSampleQuality) {
        if(presentationParameters.MultiSampleQuality <= parameters2.MultiSampleQuality) {
          return 1;
        }
        return -1;
      }
      if((this.graphics.PreferredBackBufferWidth == 0) || (this.graphics.PreferredBackBufferHeight == 0)) {
        num5 = ((float)GraphicsDeviceManager.DefaultBackBufferWidth) / ((float)GraphicsDeviceManager.DefaultBackBufferHeight);
      } else {
        num5 = ((float)this.graphics.PreferredBackBufferWidth) / ((float)this.graphics.PreferredBackBufferHeight);
      }
      float num6 = ((float)presentationParameters.BackBufferWidth) / ((float)presentationParameters.BackBufferHeight);
      float num7 = ((float)parameters2.BackBufferWidth) / ((float)parameters2.BackBufferHeight);
      float num8 = Math.Abs((float)(num6 - num5));
      float num9 = Math.Abs((float)(num7 - num5));
      if(Math.Abs((float)(num8 - num9)) > 0.2f) {
        if(num8 >= num9) {
          return 1;
        }
        return -1;
      }
      int num10 = 0;
      int num11 = 0;
      if(this.graphics.IsFullScreen) {
        if((this.graphics.PreferredBackBufferWidth == 0) || (this.graphics.PreferredBackBufferHeight == 0)) {
          GraphicsAdapter adapter = d1.Adapter;
          num10 = adapter.CurrentDisplayMode.Width * adapter.CurrentDisplayMode.Height;
          GraphicsAdapter adapter2 = d2.Adapter;
          num11 = adapter2.CurrentDisplayMode.Width * adapter2.CurrentDisplayMode.Height;
        } else {
          num10 = num11 = this.graphics.PreferredBackBufferWidth * this.graphics.PreferredBackBufferHeight;
        }
      } else if((this.graphics.PreferredBackBufferWidth == 0) || (this.graphics.PreferredBackBufferHeight == 0)) {
        num10 = num11 = GraphicsDeviceManager.DefaultBackBufferWidth * GraphicsDeviceManager.DefaultBackBufferHeight;
      } else {
        num10 = num11 = this.graphics.PreferredBackBufferWidth * this.graphics.PreferredBackBufferHeight;
      }
      int num12 = Math.Abs((int)((presentationParameters.BackBufferWidth * presentationParameters.BackBufferHeight) - num10));
      int num13 = Math.Abs((int)((parameters2.BackBufferWidth * parameters2.BackBufferHeight) - num11));
      if(num12 != num13) {
        if(num12 >= num13) {
          return 1;
        }
        return -1;
      }
      if(this.graphics.IsFullScreen && (presentationParameters.FullScreenRefreshRateInHz != parameters2.FullScreenRefreshRateInHz)) {
        if(presentationParameters.FullScreenRefreshRateInHz <= parameters2.FullScreenRefreshRateInHz) {
          return 1;
        }
        return -1;
      }
      if(d1.Adapter != d2.Adapter) {
        if(d1.Adapter.IsDefaultAdapter) {
          return -1;
        }
        if(d2.Adapter.IsDefaultAdapter) {
          return 1;
        }
      }
      return 0;
    }

    private int RankFormat(SurfaceFormat format) {
      int index = Array.IndexOf<SurfaceFormat>(GraphicsDeviceManager.ValidBackBufferFormats, format);
      if(index != -1) {
        int num2 = Array.IndexOf<SurfaceFormat>(GraphicsDeviceManager.ValidBackBufferFormats, this.graphics.PreferredBackBufferFormat);
        if(num2 == -1) {
          return (GraphicsDeviceManager.ValidBackBufferFormats.Length - index);
        }
        if(index >= num2) {
          return (index - num2);
        }
      }
      return 0x7fffffff;
    }
  }

} // namespace Nuclex
