using IfcDoc.Schema.DOC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DocExporter
{
    public class EntityNode: INotifyPropertyChanged
    {
        private readonly DocEntity _entity;
        private readonly Model _model;
        private readonly List<EntityNode> _children;
        private bool _isChecked = false;


        public EntityNode(DocEntity entity, Model model)
        {
            _entity = entity;
            _model = model;

            _children = model
                .Get<DocEntity>(e => string.Equals(_entity.Name, e.BaseDefinition, StringComparison.CurrentCultureIgnoreCase))
                .Select(e => new EntityNode(e, model))
                .OrderBy(e => e.Name)
                .ToList();
            _children.ForEach(c => c.Parent = this);
        }

        public bool IsChecked {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
                _children?.ForEach(c => c.IsChecked = value);
            }
        }

        public IEnumerable<EntityNode> Children => _children;

        public EntityNode Parent { get; private set; }

        public string Name => _entity.Name;

        public event PropertyChangedEventHandler PropertyChanged;

        public static List<EntityNode> GetRoots(Model model)
        {
            return model
                .Get<DocEntity>(e => string.IsNullOrWhiteSpace(e.BaseDefinition))
                .Select(e => new EntityNode(e, model))
                .OrderBy(e => e.Name)
                .ToList();
        }

        public static IEnumerable<EntityNode> GetFlat(IEnumerable<EntityNode> roots)
        {
            var stack = new Stack<EntityNode>(roots);
            while (stack.Count != 0)
            {
                var entity = stack.Pop();
                yield return entity;

                foreach (var e in entity.Children)
                    stack.Push(e);
            }
        }

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }


    }
}
