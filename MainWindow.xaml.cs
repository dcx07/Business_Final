using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.UI.Text;
using System.IO;
using Windows.Graphics;

namespace BusinessFinal;

public sealed partial class MainWindow : Window
{
    private const int WorkSeconds = 25 * 60;
    private const string StorageFile = "tasks.json";
    private const int MinWindowWidth = 980;
    private const int MinWindowHeight = 720;

    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
    private int _remainingSeconds = WorkSeconds;

    public ObservableCollection<TaskItem> Tasks { get; } = [];

    public MainWindow()
    {
        InitializeComponent();
        TaskListView.ItemsSource = Tasks;

        _timer.Tick += Timer_Tick;
        SizeChanged += MainWindow_SizeChanged;
        _ = LoadTasksAsync();
        RenderTimer();
    }


    private void MainWindow_SizeChanged(object sender, WindowSizeChangedEventArgs args)
    {
        var width = (int)args.Size.Width;
        var height = (int)args.Size.Height;

        if (width < MinWindowWidth || height < MinWindowHeight)
        {
            AppWindow.Resize(new SizeInt32(Math.Max(width, MinWindowWidth), Math.Max(height, MinWindowHeight)));
        }
    }

    private async void AddTask_Click(object sender, RoutedEventArgs e)
    {
        var title = TaskInput.Text.Trim();
        if (string.IsNullOrWhiteSpace(title)) return;

        Tasks.Insert(0, new TaskItem { Id = Guid.NewGuid().ToString(), Title = title, IsDone = false });
        TaskInput.Text = string.Empty;
        await SaveTasksAsync();
    }

    private async void ToggleTask_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string id }) return;
        var task = Tasks.FirstOrDefault(t => t.Id == id);
        if (task is null) return;

        task.IsDone = !task.IsDone;
        TaskListView.ItemsSource = null;
        TaskListView.ItemsSource = Tasks;
        await SaveTasksAsync();
    }

    private async void DeleteTask_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string id }) return;
        var task = Tasks.FirstOrDefault(t => t.Id == id);
        if (task is null) return;

        Tasks.Remove(task);
        await SaveTasksAsync();
    }

    private void Start_Click(object sender, RoutedEventArgs e)
    {
        if (!_timer.IsEnabled)
        {
            _timer.Start();
        }
    }

    private void Pause_Click(object sender, RoutedEventArgs e)
    {
        if (_timer.IsEnabled)
        {
            _timer.Stop();
        }
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        _timer.Stop();
        _remainingSeconds = WorkSeconds;
        RenderTimer();
    }

    private async void Timer_Tick(object? sender, object e)
    {
        _remainingSeconds--;
        if (_remainingSeconds <= 0)
        {
            _remainingSeconds = 0;
            _timer.Stop();
            var dialog = new ContentDialog
            {
                Title = "提示",
                Content = "一个番茄钟已完成，休息一下吧！",
                CloseButtonText = "好",
                XamlRoot = Content.XamlRoot
            };
            _ = await dialog.ShowAsync();
        }

        RenderTimer();
    }

    private void RenderTimer()
    {
        var minutes = _remainingSeconds / 60;
        var seconds = _remainingSeconds % 60;
        TimerText.Text = $"{minutes:00}:{seconds:00}";
    }

    private async Task LoadTasksAsync()
    {
        try
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(StorageFile);
            var json = await FileIO.ReadTextAsync(file);
            var loaded = JsonSerializer.Deserialize<List<TaskItem>>(json) ?? [];
            Tasks.Clear();
            foreach (var item in loaded)
            {
                Tasks.Add(item);
            }
        }
        catch
        {
            // Ignore if file not found or malformed.
        }
    }

    private async Task SaveTasksAsync()
    {
        var basePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BusinessFinal");
        Directory.CreateDirectory(basePath);

        var filePath = Path.Combine(basePath, StorageFile);
        var json = JsonSerializer.Serialize(Tasks);
        await File.WriteAllTextAsync(filePath, json);
    }
}

public class TaskItem
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; }

    [JsonIgnore]
    public TextDecorations Decoration => IsDone ? TextDecorations.Strikethrough : TextDecorations.None;
}
