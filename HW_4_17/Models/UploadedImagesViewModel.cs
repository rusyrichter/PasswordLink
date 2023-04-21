using PasswordLink.Data;

namespace HW_4_17.Models
{
    public class UploadedImagesViewModel
    {
        public Images Image = new Images();
      
       
        public bool Contains { get; set; }
        public string Message { get; set; }
       

        public List<int> Ids { get; set; }
       
    }
}
