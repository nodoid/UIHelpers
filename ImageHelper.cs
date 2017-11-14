namespace UiHelpers
{
    using System;
    using System.IO;
    using Android.Graphics;
    using System.Drawing;
    using Android.Content.Res;
    using Android.Content;
    using Android.Views;
    using Android.Widget;


    public static class ImageHelper
    {
        public static double RadToDeg = 180.0 / Math.PI;
        public static double DegToRad = Math.PI / 180.0;

        private static Size GetImageSizeFromArray(byte[] imgBuffer)
        {
            var options = new BitmapFactory.Options
            {
                InJustDecodeBounds = true
            };

            BitmapFactory.DecodeByteArray(imgBuffer, 0, imgBuffer.Length, options);

            return new Size(options.OutWidth, options.OutHeight);
        }

        public static int CalculateSampleSizePower2(Size originalSize, int reqWidth, int reqHeight)
        {
            int height = originalSize.Height;
            int width = originalSize.Width;
            int IMAGE_MAX_SIZE = reqWidth >= reqHeight ? reqWidth : reqHeight;

            int inSampleSize = 1;

            if (height > IMAGE_MAX_SIZE || width > IMAGE_MAX_SIZE)
            {
                inSampleSize = (int)Math.Pow(2, (int)Math.Round(Math.Log(IMAGE_MAX_SIZE /
                            (double)Math.Max(height, width)) / Math.Log(0.5)));
            }

            return inSampleSize;
        }

        private static int CalculateSampleSize(Size originalSize, int reqWidth, int reqHeight)
        {
            int sampleSize = 1;

            if (originalSize.Height > reqHeight || originalSize.Width > reqWidth)
                sampleSize = Convert.ToInt32(originalSize.Width > originalSize.Height ? 
                    (double)originalSize.Height / (double)reqHeight : (double)originalSize.Width / (double)reqWidth);

            return sampleSize;

        }

        public static Bitmap CreateUserProfileImageForDisplay(byte[] userImage, int width, int height, Resources res)
        {
            if (userImage.Length > 0 && userImage.Length != 2)
            {
                var imgSize = GetImageSizeFromArray(userImage);

                var options = new BitmapFactory.Options
                {
                    InSampleSize = CalculateSampleSizePower2(imgSize, width, height)
                };
                var scaledUserImage = BitmapFactory.DecodeByteArray(userImage, 0, userImage.Length, options);

                int scaledWidth = scaledUserImage.Width;
                int scaledHeight = scaledUserImage.Height;

                var resultImage = Bitmap.CreateScaledBitmap(BitmapFactory.DecodeResource(res, Resource.Drawable.avatarcircle), scaledWidth, scaledHeight, true);
                using (var canvas = new Canvas(resultImage))
                {
                    using (var paint = new Paint(PaintFlags.AntiAlias)
                    {
                        Dither = false,
                        FilterBitmap = true
                    })
                    {
                        paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.DstIn));
                        canvas.DrawBitmap(scaledUserImage, 0, 0, null);
                        scaledUserImage.Recycle();
								
                        using (var maskImage = Bitmap.CreateScaledBitmap(BitmapFactory.DecodeResource(res, Resource.Drawable.background), scaledWidth, scaledHeight, true))
                        {
                            canvas.DrawBitmap(maskImage, 0, 0, paint);
                            maskImage.Recycle();
                        }
                    }
                }
                return resultImage;
            }
            else
            {
                return null;
            }
        }

        public static void ResizeWidget(Button[] buttons, Context c, GravityFlags gravity)
        {
            int[] newSize = new int[2];
            float[] tSize = new float[2];
            int m = 0;
            ViewGroup.MarginLayoutParams btnParams;
            string text = "";
            float fontSize = 0f;
            foreach (var btn in buttons)
            {
                btnParams = (ViewGroup.MarginLayoutParams)btn.LayoutParameters;
                newSize = GetNewSizes(btn, c);
                text = btn.Text;
                fontSize = btn.TextSize;
				
                using (var layParams = new LinearLayout.LayoutParams(newSize[0], newSize[1]))
                {
                    layParams.SetMargins((int)ConvertDpToPixel(btnParams.LeftMargin, c), (int)ConvertDpToPixel(btnParams.TopMargin, c), 
                        (int)ConvertDpToPixel(btnParams.RightMargin, c), (int)ConvertDpToPixel(btnParams.BottomMargin, c));
                    btn.LayoutParameters = layParams;
                }
                btn.Gravity = gravity;
                tSize[m] = resizeFont(text, fontSize, btn.Width, c);
                m++;
            }
            int mr = tSize[0] > tSize[1] ? 0 : 1;

            buttons[0].SetTextSize(Android.Util.ComplexUnitType.Dip, ConvertPixelToDp(tSize[mr], c));
            buttons[1].SetTextSize(Android.Util.ComplexUnitType.Dip, ConvertPixelToDp(tSize[mr], c));
        }

		

        public static void ResizeWidget <T>(T[] buttons, Context c) where T : View
        {
            int[] newSize = new int[2];
			
            ViewGroup.MarginLayoutParams btnParams;
            foreach (T iv in buttons)
            {
                btnParams = (ViewGroup.MarginLayoutParams)iv.LayoutParameters;
                newSize = GetNewSizes(iv, c);
				
                using (var layParams = new LinearLayout.LayoutParams(newSize[0], newSize[1]))
                {
                    layParams.SetMargins((int)ConvertDpToPixel(btnParams.LeftMargin, c), (int)ConvertDpToPixel(btnParams.TopMargin, c), 
                        (int)ConvertDpToPixel(btnParams.RightMargin, c), (int)ConvertDpToPixel(btnParams.BottomMargin, c));
                    iv.LayoutParameters = layParams;
                }
            }
        }

        public static void ResizeWidget<T>(T iv, Context c) where T : View
        {
            if (iv == null)
                return;
            ViewGroup.MarginLayoutParams btnParams;
            btnParams = (ViewGroup.MarginLayoutParams)iv.LayoutParameters;
            var newSize = GetNewSizes(iv, c);
				
            using (var layParams = new LinearLayout.LayoutParams(newSize[0], newSize[1]))
            {
                layParams.SetMargins((int)ConvertDpToPixel(btnParams.LeftMargin, c), (int)ConvertDpToPixel(btnParams.TopMargin, c), 
                    (int)ConvertDpToPixel(btnParams.RightMargin, c), (int)ConvertDpToPixel(btnParams.BottomMargin, c));
                iv.LayoutParameters = layParams;
            }
        }

        public static void ResizeLayout(LinearLayout[] layout, Context c)
        {
            int[] newSize = new int[2];
			
            foreach (var ll in layout)
            {
                newSize = GetNewSizes(ll, c);
                using (var layParams = new LinearLayout.LayoutParams(newSize[0], newSize[1]))
                {
                    ll.LayoutParameters = layParams;
                }
            }
        }

        public static void ResizeLayout(LinearLayout layout, Context c)
        {
            int[] newSize = new int[2];
            using (var layParams = new LinearLayout.LayoutParams(newSize[0], newSize[1]))
            {
                layout.LayoutParameters = layParams;
            }
        }

        public static Bitmap RotateImage(string filename, bool left = false)
        { 
            Bitmap toReturn = null;
            using (var bmp = BitmapFactory.DecodeFile(filename))
            {
                if (bmp != null)
                {
                    using (var matrix = new Matrix())
                    {
                        matrix.PostRotate(!left ? 90 : -90);
                        toReturn = Bitmap.CreateBitmap(bmp, 0, 0, 
                            bmp.Width, bmp.Height, 
                            matrix, true);
				                              
                        using (var stream = new MemoryStream())
                        {
                            toReturn.Compress(Bitmap.CompressFormat.Png, 0, stream);
                            byte[] bitmapData = stream.ToArray();
                            File.WriteAllBytes(filename, bitmapData);
                        }
                    }
                }
            }
            return toReturn;
        }

        private static int[] GetNewSizes<T>(T f, Context c) where T : View
        {
            float width = f.Width == 0 ? (float)f.LayoutParameters.Width : ConvertDpToPixel((float)f.Width, c);
            float height = f.Height == 0 ? (float)f.LayoutParameters.Height : ConvertDpToPixel((float)f.Height, c);
            float xwidth = (float)Solution.ScreenX;
            float yheight = (float)Solution.ScreenY;
            int[] size = new int[2];
            size[0] = (int)((width / 480) * xwidth);
            size[1] = (int)((height / 800) * yheight);      
            return size;        
        }

        public static float[] GetNewSizes(float[] sizes, Context c)
        {
            float xSize = ConvertDpToPixel(sizes[0], c);
            float ySize = ConvertDpToPixel(sizes[1], c);
            float xwidth = (float)Solution.ScreenX;
            float yheight = (float)Solution.ScreenY;
            float[] finalSize = new float[2];
            finalSize[0] = ConvertPixelToDp(((xSize / 480) * xwidth), c);
            finalSize[1] = ConvertPixelToDp(((ySize / 800) * yheight), c);
            return finalSize;
        }

        public static ImageView RotateImage(ImageView image, RectangleF origImageFrame, RectangleF rotatedImageRect, double degAngle, bool disposeOriginalImage)
        {
            ImageView toReturn = image;
            float width = image.Drawable.Bounds.Width() / 2;
            float height = image.Drawable.Bounds.Height() / 2;
            var matrix = new Matrix();
            toReturn.SetScaleType(ImageView.ScaleType.Matrix); 
            matrix.PostRotate((float)degAngle, width, height);
            toReturn.ImageMatrix = matrix;
            return toReturn;
        }

        public static ImageView CropImage(ImageView image, RectangleF cropArea, Context c)
        {
            Bitmap mBitmap = null;
            var croppedImage = Bitmap.CreateBitmap((int)cropArea.Width, (int)cropArea.Height,
                                   Bitmap.Config.Argb8888);
            var canvas = new Canvas(croppedImage);
            var dstRect = new Rect(0, 0, (int)cropArea.Width, (int)cropArea.Height);
            var croppedArea = new Rect((int)cropArea.Left, (int)cropArea.Top, (int)cropArea.Right, (int)cropArea.Bottom);
            canvas.DrawBitmap(mBitmap, croppedArea, dstRect, null);
            var v = new ImageView(c);
            v.SetImageBitmap(mBitmap);
            return v;
        }


        public static RectangleF TranslateRect(RectangleF fromRect, RectangleF rect, SizeF toSize)
        {
            float rectSx = toSize.Width / fromRect.Width;
            float rectSy = toSize.Height / fromRect.Height;
            float x = rect.X * rectSx;
            float y = rect.Y * rectSy;
            float width = rect.Width * rectSx;
            float height = rect.Height * rectSy;
            return new RectangleF(x, y, width, height);
        }


        private static float resizeFont(string text, float size, float btnWidth, Context context)
        {
            var paint = new Paint(PaintFlags.AntiAlias);
            paint.TextSize = size;
            paint.SetTypeface(Typeface.DefaultBold);
            float width = ConvertDpToPixel(paint.MeasureText(text), context);
            float pxWidth = ConvertDpToPixel(btnWidth, context);
            while (width <= pxWidth)
            {
                size += .5f;
                paint.TextSize = size;
                width = ConvertDpToPixel(paint.MeasureText(text), context);
            }
			
            return size;
        }

        public static float GetNewFontSize(float size, Context c)
        {
            float fSize = ConvertDpToPixel(size, c);
            float calc = fSize + (fSize * .1f);
            return ConvertPixelToDp(calc, c);
        }

        public static float ConvertDpToPixel(float dp, Context context)
        {
            var metrics = context.Resources.DisplayMetrics;
            return dp * ((float)metrics.DensityDpi / 160f);
        }

        public static float ConvertPixelToDp(float px, Context context)
        {
            var metrics = context.Resources.DisplayMetrics;
            return (px * 160f) / (float)metrics.DensityDpi;
        }
    }
}

