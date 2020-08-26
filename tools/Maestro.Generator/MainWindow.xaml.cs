using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;

namespace Maestro.Generator
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string FileFilter = "Maestro Database|*.mtrdb";
        private const string LastFileConfig = "LastFile";
        private readonly Configuration configuration;
        private readonly Stack<object> undoes = new Stack<object>();
        private readonly Stack<object> redoes = new Stack<object>();
        private bool disableUndo = false;


        public DataContext Context { get; set; } = new DataContext();

        public MainWindow(string filename = null)
        {
            configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            InitializeComponent();

            if (!string.IsNullOrWhiteSpace(filename) && File.Exists(filename))
            {
                Context.Load(filename);
                UpdateSettings();
            }
            else if (configuration.AppSettings.Settings.AllKeys.Contains(LastFileConfig) && File.Exists(configuration.AppSettings.Settings[LastFileConfig].Value))
                Context.Load(ConfigurationManager.AppSettings.Get(LastFileConfig));


            ModelBase.OnChanging += ModelBase_OnChanging;
            DataContext = Context;
        }

        private void ModelBase_OnChanging(object delta)
        {
            if (!disableUndo)
            {
                undoes.Push(delta);
                redoes.Clear();
            }

            switch (delta)
            {
                case ModelBase.PropertyChange change:
                    LogsTxt.AppendText($"{change.Origin} changed value of {change.PropertyName} ({change.Type.Name}) from {change.PreviousValue} to {change.NewValue}{Environment.NewLine}");
                    break;
                case ModelBase.ListAddition addition:
                    LogsTxt.AppendText($"{addition.Added} added to {addition.ListName()}{Environment.NewLine}");
                    break;
                case ModelBase.ListSuppression suppression:
                    LogsTxt.AppendText($"{suppression.Suppressed} removed from {suppression.ListName()}{Environment.NewLine}");
                    break;
                default:
                    throw new Exception();
            }
            LogsTxt.ScrollToEnd();
        }

        private void ResetLogs()
        {
            LogsTxt.Clear();
            undoes.Clear();
            redoes.Clear();
        }

        private void New(object sender, RoutedEventArgs e)
        {
            Context.Reset();
            ResetLogs();
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Filter = FileFilter };
            var result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                Context.Load(openFileDialog.FileName);
                UpdateSettings();
                ResetLogs();
            }
        }
        private void Save(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Context.FileName))
                SaveAs(this, e);
            else
                Context.Save();
        }
        private void SaveAs(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog { Filter = FileFilter };
            var result = saveFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                Context.Save(saveFileDialog.FileName);
                UpdateSettings();
            }
        }
        private void Quit(object sender, RoutedEventArgs e)
        {
            Save(sender, e);
            Environment.Exit(0);
        }

        private void UpdateSettings()
        {
            configuration.AppSettings.Settings.Remove(LastFileConfig);
            configuration.AppSettings.Settings.Add(LastFileConfig, Context.FileName);
            configuration.Save();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Save(this, new RoutedEventArgs());
            base.OnClosing(e);
        }

        private void Generate(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Context.FileName))
                Save(sender, e);

            Context.Generate();
        }

        private void Undo(object sender, RoutedEventArgs e)
        {
            if(undoes.Count != 0)
            {
                var delta = undoes.Pop();
                disableUndo = true;
                switch(delta)
                {
                    case ModelBase.PropertyChange change:
                        change.Undo();
                        redoes.Push(change);
                        break;
                    case ModelBase.ListAddition addition:
                        disableUndo = true;
                        addition.Undo();
                        redoes.Push(addition.Invert());
                        disableUndo = false;
                        break;
                    case ModelBase.ListSuppression suppression:
                        disableUndo = true;
                        suppression.Undo();
                        redoes.Push(suppression.Invert());
                        disableUndo = false;
                        break;
                    default:
                        throw new Exception();
                }
                disableUndo = false;
            }
        }

        private void Redo(object sender, RoutedEventArgs e)
        {
            if(redoes.Count != 0)
            {
                var delta = redoes.Pop();
                switch (delta)
                {
                    case ModelBase.PropertyChange change:
                        change.Redo();
                        break;
                    case ModelBase.ListAddition addition:
                        disableUndo = true;
                        addition.Undo();
                        undoes.Push(addition.Invert());
                        disableUndo = false;
                        break;
                    case ModelBase.ListSuppression suppression:
                        disableUndo = true;
                        suppression.Undo();
                        undoes.Push(suppression.Invert());
                        disableUndo = false;
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

    }
}
