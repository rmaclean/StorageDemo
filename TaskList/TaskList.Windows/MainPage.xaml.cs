using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TaskList
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // this is where we will keep the tasks in memory
        public ObservableCollection<string> Tasks { get; set; }
        private XDocument xmlDocument = new XDocument();

        public MainPage() // this is our constructor
        {
            this.InitializeComponent();
            // here is where we create the collection
            Tasks = new ObservableCollection<string>();

            //// still being lazy
            //var settings = Windows.Storage.ApplicationData.Current.LocalSettings;

            //// loop over all the settings we have
            //foreach (var setting in settings.Values)
            //{
            //    // add each value to the task list
            //    Tasks.Add((string)setting.Value);
            //}

            // here is where we associate the collection with screen
            // so the screen can display it
            this.DataContext = Tasks;
        }

        // everytime I navigate to this page, this runs
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // still lazy
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;

            try // if the file doesn't exist the try..catch will just return
            {
                 // get the file
                var file = await folder.GetFileAsync("tasks.xml");
                
                // do not need this with a global variable
                //// place to put the xml in memory
                //var xmldocument = new XDocument();

                // open file for read
                using (var fileStream = await file.OpenStreamForReadAsync())
                {
                    // read from file
                    xmlDocument = XDocument.Load(fileStream);
                }

                // loop over the task elements in the XML
                foreach (var task in xmlDocument.Descendants("task"))
                {
                    // add the values to the in memory structure
                    Tasks.Add(task.Value);
                }
            }
            catch
            {
                return;
            }     
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            Tasks.Add(textBox.Text); // add the content of the text box as
                                     // an item to our collection

            //// because I do not want to type out the long path each time
            //// I assign it to a variable
            //var settings = Windows.Storage.ApplicationData.Current.LocalSettings;

            //// adding a task to the setttings
            //// I use a unique ID for the key
            //settings.Values.Add(Guid.NewGuid().ToString(), textBox.Text);

            // create a new document
            //var xmlDocument = new XDocument();
            // create a root element for it
            if (xmlDocument.Root == null)
            {
                var root = new XElement("tasks");
                // add root element to the document
                xmlDocument.Add(root);
            }

            // create a leaf element for the task
            var newTask = new XElement("task");

            // set the value of the leaf element
            newTask.Value = textBox.Text;

            // add the leaf to the root
            xmlDocument.Root.Add(newTask);           

            // so I do not need to type out the full path
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            // create a file
            var file = await folder.CreateFileAsync("tasks.xml", Windows.Storage.CreationCollisionOption.ReplaceExisting);

            // open the file so we can write to it
            using (var fileStream = await file.OpenStreamForWriteAsync())
            {
                xmlDocument.Save(fileStream); // write from our XML in memory
                                              // to the file
            } // close the file so it is saved

           textBox.Text = ""; // clear the textbox
        }
    }
}
