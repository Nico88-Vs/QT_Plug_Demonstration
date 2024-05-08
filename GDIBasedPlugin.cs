using System.Collections.Generic;
using System.Drawing;
using TradingPlatform.BusinessLayer;
using TradingPlatform.PresentationLayer.Plugins;

namespace Test_Api_Connection
{
    public class Test_Api_Connection : Plugin
    {
        private GdiRenderer gdiRenderer;
        private Connections cnn;
        public static readonly string postwithbody = "http://localhost:5000/sample?message=";
        public static readonly string req_type = "http://localhost:5000/sample/get-message";

        /// <summary>
        /// Plugin meta information
        /// </summary>
        public static PluginInfo GetInfo()
        {
            var windowParameters = NativeWindowParameters.Panel;
            windowParameters.AllowDrop = true;
            windowParameters.BrowserUsageType = BrowserUsageType.None;

            return new PluginInfo()
            {
                Name = "Test_Api_Connection",
                Title = "Test_Api_Connection",
                Group = PluginGroup.Misc,
                ShortName = "T_API",
                SortIndex = 35,
                AllowSettings = true,
                WindowParameters = windowParameters,
                CustomProperties = new Dictionary<string, object>()
                {
                    {PluginInfo.Const.ALLOW_MANUAL_CREATION, true }
                }
            };
        }

        public override Size DefaultSize => new Size(this.UnitSize.Width * 1, this.UnitSize.Height * 2);

        public override void Initialize()
        {
            base.Initialize();

            this.cnn = new Connections();
            this.gdiRenderer = new GdiRenderer(this.Window.CreateRenderingControl("GdiRenderer"), this.cnn);

            this.cnn.message_recived += this.Cnn_message_recived;
        }

        private void Cnn_message_recived(object sender, string e)
        {
            this.gdiRenderer.Text = e;
            this.gdiRenderer.RedrawBufferedGraphic();

            if (e == "symbol")
            {
                cnn.StartGetPats($"{postwithbody}{Core.Instance.Symbols[0].Name}");
            }

            if (e == "balance")
            {
                cnn.StartGetPats($"{postwithbody}{Core.Instance.Accounts[0].Balance}");
            }
        }

        public override void Populate(PluginParameters args = null)
        {
            base.Populate(args);

            // Redraw renderer
            this.gdiRenderer.RedrawBufferedGraphic();
        }

        public override void Dispose()
        {
            if (this.gdiRenderer != null)
            {
                this.gdiRenderer.Dispose();
                this.gdiRenderer = null;
            }

            base.Dispose();
        }

        public override IList<SettingItem> Settings
        {
            get
            {
                var result = base.Settings;

                // Here you can specify customf settings for your plugin
                result.Add(new SettingItemColor("Color", this.gdiRenderer.Color));

                return result;
            }
            set
            {
                base.Settings = value;

                // Apply custom settings
                if (value.GetItemByPath("Color") is SettingItemColor color)
                {
                    this.gdiRenderer.Color = (Color)color.Value;
                    this.gdiRenderer.RedrawBufferedGraphic();
                }
            }
        }

        protected override void OnLayoutUpdated()
        {
            base.OnLayoutUpdated();

            if (this.gdiRenderer != null)
                this.gdiRenderer.Layout.Margin = this.NonClientMargin;
        }
    }
}
