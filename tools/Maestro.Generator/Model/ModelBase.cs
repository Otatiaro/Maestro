using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maestro.Generator
{
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            OnChanging?.Invoke(new PropertyChange(this, propertyName, typeof(T), field, value));
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        public static void Add<T>(ObservableCollection<T> list, T value)
        {
            list.Add(value);
            OnChanging?.Invoke(new ListAddition(list, typeof(T), value));
        }
        public static void Remove<T>(ObservableCollection<T> list, T value)
        {
            list.Remove(value);
            OnChanging?.Invoke(new ListSuppression(list, typeof(T), value));
        }

        public static Dictionary<object, string> ListNames { get; } = new Dictionary<object, string>();

        public struct PropertyChange
        {
            public PropertyChange(ModelBase origin, string propertyName, Type type, object previousValue, object newValue)
            {
                Origin = origin;
                PropertyName = propertyName;
                Type = type;
                PreviousValue = previousValue;
                NewValue = newValue;
            }

            public ModelBase Origin { get; }

            public string PropertyName { get; }

            public Type Type { get; }

            public object PreviousValue { get; }

            public object NewValue { get; }

            public void Undo()
            {
                Origin.GetType().GetProperty(PropertyName, Type).SetValue(Origin, PreviousValue);
            }

            public void Redo()
            {
                Origin.GetType().GetProperty(PropertyName, Type).SetValue(Origin, NewValue);
            }
        }

        public struct ListAddition
        {
            public object List;
            public Type Type;
            public object Added;

            public ListAddition(object list, Type type, object added)
            {
                List = list;
                Type = type;
                Added = added;
            }

            public ListSuppression Invert() => new ListSuppression(List, Type, Added);

            public void Undo()
            {
                List.GetType().GetMethod("Remove", new[] { Type }).Invoke(List, new[] { Added }); ;
                OnChanging?.Invoke(new ListSuppression(List, Type, Added));
            }

            public string ListName() => ListNames.ContainsKey(List) ? ListNames[List] : $"{List.GetType().Name}<{Type.Name}>";
        }

        public struct ListSuppression
        {
            public object List;
            public Type Type;
            public object Suppressed;

            public ListSuppression(object list, Type type, object suppressed)
            {
                List = list;
                Type = type;
                Suppressed = suppressed;
            }

            public ListAddition Invert() => new ListAddition(List, Type, Suppressed);

            public void Undo()
            {
                List.GetType().GetMethod("Add", new[] { Type }).Invoke(List, new[] { Suppressed }); ;
                OnChanging?.Invoke(new ListAddition(List, Type, Suppressed));
            }
            public string ListName() => ListNames.ContainsKey(List) ? ListNames[List] : $"{List.GetType().Name}<{Type.Name}>";
        }

        public delegate void OnChangingEventHandler(object change);
        public static event OnChangingEventHandler OnChanging;
    }
}
