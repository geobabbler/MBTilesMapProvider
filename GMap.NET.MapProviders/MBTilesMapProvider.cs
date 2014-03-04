/*
The MIT License (MIT)

Copyright (c) 2014 William Dollins (bill@geomusings.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 */


namespace GMap.NET.MapProviders
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Text;
    using GMap.NET.Projections;
    using MapTiler.GlobalMapTiles;
    using MBTilesSupport.Helpers;

    public abstract class MBTilesMapProviderBase : GMapProvider
    {

        #region GMapProvider Members
        public override Guid Id
        {
            get { throw new NotImplementedException(); }
        }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override PureProjection Projection
        {
            get { return GMap.NET.Projections.MercatorProjection.Instance; }
        }

        GMapProvider[] overlays;
        public override GMapProvider[] Overlays
        {
            get
            {
                if (overlays == null)
                {
                    overlays = new GMapProvider[] { this };
                }
                return overlays;
            }
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    
    public class MBTilesMapProvider : MBTilesMapProviderBase
    {
        internal static PureImageProxy ImageProxy;

        private MBTilesHelper _mbtiles = null;


        public MBTilesMapProvider(string dbpath)
        {
            _mbtiles = new MBTilesHelper(dbpath);
            MaxZoom = _mbtiles.MaxZoom;
            MinZoom = _mbtiles.MinZoom;
            InvertedAxisY = true;
            Area = new RectLatLng(_mbtiles.Bounds.West, _mbtiles.Bounds.North, _mbtiles.Bounds.East, _mbtiles.Bounds.South);
        }

        public PointLatLng Center
        {
            get
            {
                return new PointLatLng(_mbtiles.Center.Y, _mbtiles.Center.X);
            }
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("EDA4CAAB-0505-4D82-9E64-72F13A66170A");
        public override Guid Id
        {
            get
            {
                return id;
            }
        }

        public override string Name
        {
            get
            {
                return _mbtiles.Name;
            }
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            //string url = MakeTileImageUrl(pos, zoom, LanguageStr);

            return getImage(pos, zoom);
        }

        private PureImage getImage(GPoint pos, int zoom)
        {
            PureImage retval = null ;

            var resultImage = _mbtiles.GetTileStream(pos.X, pos.Y, zoom);
            
                if (resultImage.Length > 0)
                {
                    //resultImage.Position = 0L;
                    retval = ImageProxy.FromArray(resultImage);
                }
                //var stream = new MemoryStream();
                //resultImage.Save(stream, ImageFormat.Png);
                //stream.Position = 0L;
                //retval.Data = stream;
            
            //var img = _mbtiles.GetTile(pos.X, pos.Y, zoom);
            //img.Position = 0L;
            
            //retval.Data = img;

            return retval;
        }
        #endregion
    }

}
