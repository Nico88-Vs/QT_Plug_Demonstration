using QuantowerPlugin_Decomplied;
using System;
using System.Drawing;
using System.Linq;
using TradingPlatform.BusinessLayer;
using TradingPlatform.BusinessLayer.Native;
using TradingPlatform.PresentationLayer.Plugins;
using TradingPlatform.PresentationLayer.Renderers;
using static System.Net.Mime.MediaTypeNames;

namespace Test_Api_Connection
{
    public class GdiRenderer : Renderer
    {
        #region Proeprties

        private readonly BufferedGraphic bufferedGraphic;

        public Color Color { get; set; }
        public string Text { get; set; } = "Start";
        public Rectangle bound { get; set; } = new Rectangle(10, 10, 50, 30);
        public Connections Cnn { get; set; }
        public Consumer Consumer { get; set; }

        #endregion

        public GdiRenderer(IRenderingNativeControl native, Connections cnn)
           : base(native)
        {
            // Subscribe to mouse events
            native.MouseDownNative += this.OnMouseDown;
            native.MouseUpNative += this.OnMouseUp;
            native.MouseMoveNative += this.OnMouseMove;

            this.Cnn = cnn;
            this.Cnn.message_recived += this.Cnn_message_recived;
            this.Consumer = new Consumer();

            this.Color = Color.RebeccaPurple;
            this.bufferedGraphic = new BufferedGraphic(this.Draw, this.Refresh, native.DisposeImage, native.IsDisplayed, requiredThreadType: BufferedGraphicRequiredThreadType.LowPriority);
        }

        private void Cnn_message_recived(object sender, string e)
        {
           
        }

        public void RedrawBufferedGraphic()
        {
            this.bufferedGraphic.IsDirty = true;
        }

        /// <summary>
        /// Implement your painting in this method
        /// </summary>
        protected virtual void Draw(Graphics graphics)
        {
            graphics.Clear(Color.White);


            // Disegna lo sfondo del bottone
            graphics.FillRectangle(Theme_.Brash_Backcground_Button, bound);
            graphics.DrawRectangle(Theme_.BorderPen, bound);

            // Disegna il testo del bottoneb
            SizeF textSize = graphics.MeasureString(Text, Theme_.Font);
            PointF textPosition = new PointF(bound.X + (bound.Width - textSize.Width) / 2,
                                             bound.Y + (bound.Height - textSize.Height) / 2);

            // Crea un rettangolo di disegno che limita il testo all'interno dei Bounds
            RectangleF textRect = new RectangleF(
                bound.X,
                bound.Y,
                bound.Width,
                bound.Height
            );

            // Utilizza il rettangolo di clipping per disegnare il testo
            // Questo assicura che il testo non esci dai confini definiti
            graphics.SetClip(textRect);

            graphics.DrawString(Text, Theme_.Font, Theme_.Text_brush, textPosition);

            graphics.ResetClip();
        }

        public override nint Render() => bufferedGraphic.CurrentImage;

        public override void Dispose()
        {
            if (this.bufferedGraphic != null)
                this.bufferedGraphic.Dispose();

            base.Dispose();
        }

        public override void OnResize()
        {
            base.OnResize();

            Rectangle bounds = this.Bounds;
            if (bounds.Width == 0 || bounds.Height == 0)
                return;

            try
            {
                //
                // Recreate buffer
                //
                bufferedGraphic.Resize(bounds.Width, bounds.Height);
                bufferedGraphic.IsDirty = true;
            }
            catch (Exception ex)
            { Console.WriteLine("Error during resizing: " + ex.Message); }
        }

        #region Mouse Processing

        private void OnMouseDown(NativeMouseEventArgs e)
        {
            // Add your MouseDown processing logic here
        }

        private void OnMouseMove(NativeMouseEventArgs obj)
        {
            // Add your MouseMove processing logic here
        }

        private void OnMouseUp(NativeMouseEventArgs obj)
        {
            if (this.bound.Contains(obj.Location))
            {
                this.Cnn.StartGetPats("http://localhost:5000/sample/get-message");
            }
        }

        #endregion
    }
}
