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


using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Text;
using MapTiler.GlobalMapTiles;

namespace MBTilesSupport.Helpers
{
    public class MBTilesHelper
    {
        public GeoExtent Bounds { get; private set; }
        public CoordinatePair Center { get; private set; }
        public int MinZoom { get; private set; }
        public int MaxZoom { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string MBTilesVersion { get; private set; }
        public string Path { get; private set; }

        private GlobalMercator gmt = new GlobalMercator();

        public MBTilesHelper(string path)
        {
            this.Path = path;
            loadMetadata();
        }

        private void loadMetadata()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(String.Format("Data Source={0};Version=3;", this.Path)))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand() { Connection = conn, CommandText = "SELECT * FROM metadata;" })
                    {
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string name = reader["name"].ToString();
                            switch (name.ToLower())
                            {
                                case "bounds":
                                    string val = reader["value"].ToString();
                                    string[] vals = val.Split(new char[] { ',' });
                                    this.Bounds = new GeoExtent() { West = Convert.ToDouble(vals[0]), South = Convert.ToDouble(vals[1]), East = Convert.ToDouble(vals[2]), North = Convert.ToDouble(vals[3]) };
                                    break;
                                case "center":
                                    val = reader["value"].ToString();
                                    vals = val.Split(new char[] { ',' });
                                    this.Center = new CoordinatePair() { X = Convert.ToDouble(vals[0]), Y = Convert.ToDouble(vals[1]) };
                                    break;
                                case "minzoom":
                                    this.MinZoom = Convert.ToInt32(reader["value"]);
                                    break;
                                case "maxzoom":
                                    this.MaxZoom = Convert.ToInt32(reader["value"]);
                                    break;
                                case "name":
                                    this.Name = reader["value"].ToString();
                                    break;
                                case "description":
                                    this.Description = reader["value"].ToString();
                                    break;
                                case "version":
                                    this.MBTilesVersion = reader["value"].ToString();
                                    break;

                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public Image GetTileLatLng(double latitude, double longtiude, int zoom)
        {
            Image retval = null;
            var llt = gmt.LatLonToTile(latitude, longtiude, zoom);
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(String.Format("Data Source={0};Version=3;", Path)))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand() { Connection = conn, CommandText = String.Format("SELECT * FROM tiles WHERE tile_column = {0} and tile_row = {1} and zoom_level = {2};", llt.X, llt.Y, zoom) })
                    {
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            byte[] bytes = reader["tile_data"] as byte[];
                            using (MemoryStream stream = new MemoryStream(bytes))
                            {
                                retval = Image.FromStream(stream);
                            }
                        }
                    }
                }
            }
            catch
            {
                retval = null;
            }
            return retval;
        }

        public byte[] GetTileStreamLatLng(double latitude, double longtiude, int zoom)
        {
            byte[] retval = null;
            var llt = gmt.LatLonToTile(latitude, longtiude, zoom);
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(String.Format("Data Source={0};Version=3;", Path)))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand() { Connection = conn, CommandText = String.Format("SELECT * FROM tiles WHERE tile_column = {0} and tile_row = {1} and zoom_level = {2};", llt.X, llt.Y, zoom) })
                    {
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            byte[] bytes = reader["tile_data"] as byte[];
                            retval = bytes;
                        }
                    }
                }
            }
            catch
            {
                retval = null;
            }
            return retval;
        }

        public Image GetTile(long x, long y, int zoom)
        {
            Image retval = null;
            //var llt = gmt.LatLonToTile(latitude, longtiude, zoom);
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(String.Format("Data Source={0};Version=3;", Path)))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand() { Connection = conn, CommandText = String.Format("SELECT * FROM tiles WHERE tile_column = {0} and tile_row = {1} and zoom_level = {2};", x, y, zoom) })
                    {
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            byte[] bytes = reader["tile_data"] as byte[];
                            using (MemoryStream stream = new MemoryStream(bytes))
                            {
                                retval = Image.FromStream(stream);
                            }
                        }
                    }
                }
            }
            catch
            {
                retval = null;
            }
            return retval;
        }

        public byte[] GetTileStream(long x, long y, int zoom)
        {
            byte[] retval = null;
            //var llt = gmt.LatLonToTile(latitude, longtiude, zoom);
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(String.Format("Data Source={0};Version=3;", Path)))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand() { Connection = conn, CommandText = String.Format("SELECT * FROM tiles WHERE tile_column = {0} and tile_row = {1} and zoom_level = {2};", x, y, zoom) })
                    {
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            byte[] bytes = reader["tile_data"] as byte[];
                            retval = bytes; // new MemoryStream(bytes);
                            //using (MemoryStream stream = new MemoryStream(bytes))
                            //{
                            //    retval = stream;
                            //}
                        }
                    }
                }
            }
            catch
            {
                retval = null;
            }
            return retval;
        }
    }
}
