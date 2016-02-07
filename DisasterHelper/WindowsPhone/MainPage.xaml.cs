using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WindowsPhone.Resources;
using Microsoft.WindowsAzure.MobileServices;
using Common.Tables;
using Common;
using Microsoft.Phone.Tasks;
using Common.Tables.Disaster;
using Common.Utils;
using System.Windows.Media.Imaging;

namespace WindowsPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        // MobileServiceCollectionView implements ICollectionView (useful for databinding to lists) and 
        // is integrated with your Mobile Service to make it easy to bind your data to the ListView
        private MobileServiceCollection<TodoItem, TodoItem> items;
        private IMobileServiceTable<TodoItem> todoTable = Service.GetTable<TodoItem>();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void InsertTodoItem(TodoItem todoItem)
        {
            await Service.InsertItemAsync(todoItem);
            items.Add(todoItem);
        }

        private async void RefreshTodoItems()
        {
            try
            {
                items = await todoTable
                    .Where(todoItem => todoItem.Complete == false)
                    .ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                MessageBox.Show(e.Message, "Error loading items", MessageBoxButton.OK);
            }

            //ListItems.ItemsSource = items;
        }

        private async void UpdateCheckedTodoItem(TodoItem item)
        {
            await todoTable.UpdateAsync(item);
            items.Remove(item);
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshTodoItems();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            //var todoItem = new TodoItem { Text = TodoInput.Text };
            //InsertTodoItem(todoItem);
        }

        private void CheckBoxComplete_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TodoItem item = cb.DataContext as TodoItem;
            item.Complete = true;
            UpdateCheckedTodoItem(item);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //var todoItem = new TodoItem { Text = "adas" };
            //InsertTodoItem(todoItem);
            //RefreshTodoItems();
        }


        private void btnStartCamera_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptureTask camera = new CameraCaptureTask();
            camera.Completed += camera_Completed;
            camera.Show();
        }

        async void camera_Completed(object sender, PhotoResult e)
        {
            var table = Service.GetTable<Missing>();
            var image = ImageHelper.GetFileBytes(e.ChosenPhoto);
            var missing = new Missing
            {
                Description = "",
                Feedback = "",
                Found = true,
                Name = "",
                UserPostingId = "asdsa",
                Image = image
            };
            await table.InsertAsync(missing);
        }

        private async void btnLoadLastImage_Click(object sender, RoutedEventArgs e)
        {
            var table = await Service.GetTable<Missing>().ToListAsync();
            var image = table.Last().Image;

            var bmp = new BitmapImage();
            bmp.SetSource(ImageHelper.GetFileStream(image));
            resultImage.Source = bmp;
        }
    }
}