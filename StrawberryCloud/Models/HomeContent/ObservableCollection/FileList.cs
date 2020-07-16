using System.Windows.Input;

namespace StrawberryCloud.Models.HomeContent.ObservableCollection
{
    class FileList
    {
        public string name { get; set; }
        public string type { get; set; }
        public string size { get; set; }
        public ICommand clickCommand { get; set; }
        public ICommand deleteCommand { get; set; }
    }
}
