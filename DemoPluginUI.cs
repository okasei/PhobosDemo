using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PhobosDemo
{
    /// <summary>
    /// 插件 UI 示例
    /// </summary>
    public class DemoPluginUI : UserControl
    {
        private readonly DemoPlugin _plugin;

        // UI 组件
        private TextBox? _inputTextBox;
        private TextBlock? _outputTextBlock;
        private ListBox? _logListBox;

        public DemoPluginUI(DemoPlugin plugin)
        {
            _plugin = plugin;
            InitializeUI();
        }

        private void InitializeUI()
        {
            // 主容器
            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // ===== 输入区域 =====
            var inputPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(10)
            };

            var inputLabel = new TextBlock
            {
                Text = "Input: ",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 5, 0)
            };

            _inputTextBox = new TextBox
            {
                Width = 200,
                Margin = new Thickness(0, 0, 10, 0)
            };

            var executeButton = new Button
            {
                Content = "Execute",
                Padding = new Thickness(15, 5, 15, 5)
            };
            executeButton.Click += OnExecuteClick;

            inputPanel.Children.Add(inputLabel);
            inputPanel.Children.Add(_inputTextBox);
            inputPanel.Children.Add(executeButton);

            Grid.SetRow(inputPanel, 0);
            mainGrid.Children.Add(inputPanel);

            // ===== 测试按钮区域 =====
            var buttonPanel = new WrapPanel
            {
                Margin = new Thickness(10, 0, 10, 10)
            };

            var testButtons = new (string Text, Action Handler)[]
            {
                ("Test Config", OnTestConfigClick),
                ("Test Boot Item", OnTestBootItemClick),
                ("Test System Info", OnTestSystemInfoClick),
                ("Test Hello", OnTestHelloClick),
                ("Test Calculate", OnTestCalculateClick),
                ("Clear Log", OnClearLogClick)
            };

            foreach (var (text, handler) in testButtons)
            {
                var button = new Button
                {
                    Content = text,
                    Padding = new Thickness(10, 5, 10, 5),
                    Margin = new Thickness(0, 0, 5, 5)
                };
                button.Click += (s, e) => handler();
                buttonPanel.Children.Add(button);
            }

            Grid.SetRow(buttonPanel, 1);
            mainGrid.Children.Add(buttonPanel);

            // ===== 日志区域 =====
            var logBorder = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(10, 0, 10, 10)
            };

            _logListBox = new ListBox
            {
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                Foreground = Brushes.LightGray,
                FontFamily = new FontFamily("Consolas")
            };

            logBorder.Child = _logListBox;
            Grid.SetRow(logBorder, 2);
            mainGrid.Children.Add(logBorder);

            // ===== 输出区域 =====
            var outputPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(10, 0, 10, 10)
            };

            var outputLabel = new TextBlock
            {
                Text = "Output: ",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 5, 0)
            };

            _outputTextBlock = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Brushes.Green
            };

            outputPanel.Children.Add(outputLabel);
            outputPanel.Children.Add(_outputTextBlock);

            Grid.SetRow(outputPanel, 3);
            mainGrid.Children.Add(outputPanel);

            // 设置内容
            Content = mainGrid;

            // 记录初始化日志
            Log("Plugin UI initialized");
        }

        #region 事件处理

        private async void OnExecuteClick(object sender, RoutedEventArgs e)
        {
            var input = _inputTextBox?.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(input))
            {
                Log("Please enter a command");
                return;
            }

            Log($"Executing: {input}");

            try
            {
                var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var command = parts[0];
                var args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

                var result = await _plugin.Run(command, args);

                SetOutput(result.Message);
                Log($"Result: {result.Success} - {result.Message}");

                if (result.Data.Count > 0)
                {
                    foreach (var data in result.Data)
                    {
                        Log($"  Data: {data}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
                SetOutput($"Error: {ex.Message}");
            }
        }

        private async void OnTestConfigClick()
        {
            Log("Testing config read/write...");
            try
            {
                var result = await _plugin.TestConfigReadWrite("TestKey", $"TestValue_{DateTime.Now:HHmmss}");
                Log(result);
                SetOutput(result);
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }

        private async void OnTestBootItemClick()
        {
            Log("Testing boot item...");
            try
            {
                var result = await _plugin.TestBootItem();
                Log(result);
                SetOutput(result);
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }

        private async void OnTestSystemInfoClick()
        {
            Log("Getting system info...");
            try
            {
                var result = await _plugin.TestGetSystemInfo();
                foreach (var line in result.Split('\n'))
                {
                    Log(line);
                }
                SetOutput(result.Replace('\n', ' '));
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }

        private async void OnTestHelloClick()
        {
            var name = _inputTextBox?.Text ?? "World";
            Log($"Calling hello with: {name}");
            try
            {
                var result = await _plugin.Run("hello", name);
                Log(result.Message);
                SetOutput(result.Message);
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }

        private async void OnTestCalculateClick()
        {
            Log("Calculating 10 + 20...");
            try
            {
                var result = await _plugin.Run("calculate", "10", "20");
                Log(result.Message);
                SetOutput(result.Message);
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
        }

        private void OnClearLogClick()
        {
            _logListBox?.Items.Clear();
            SetOutput(string.Empty);
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 设置上次输入
        /// </summary>
        public void SetLastInput(string value)
        {
            if (_inputTextBox != null)
            {
                _inputTextBox.Text = value;
            }
        }

        /// <summary>
        /// 获取当前输入
        /// </summary>
        public string GetCurrentInput()
        {
            return _inputTextBox?.Text ?? string.Empty;
        }

        /// <summary>
        /// 设置输出
        /// </summary>
        public void SetOutput(string text)
        {
            if (_outputTextBlock != null)
            {
                _outputTextBlock.Text = text;
            }
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        public void Log(string message)
        {
            if (_logListBox == null) return;

            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            _logListBox.Items.Add($"[{timestamp}] {message}");

            // 自动滚动到底部
            if (_logListBox.Items.Count > 0)
            {
                _logListBox.ScrollIntoView(_logListBox.Items[^1]);
            }

            // 限制日志条数
            while (_logListBox.Items.Count > 100)
            {
                _logListBox.Items.RemoveAt(0);
            }
        }

        #endregion
    }
}