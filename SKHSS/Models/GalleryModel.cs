using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKHSS.Models
{
    public class GalleryModel
    {
        public List<Album> MyAlbumList { get; set; }
        public List<List<Picture>> TeamsList = new List<List<Picture>>();
        public List<Album> PublicAlbums { get; set; }
        public GalleryModel()
        {
        }
    }
}