using BuildingSmart.Serialization.Step;
using IfcDoc.Schema;
using IfcDoc.Schema.DOC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocExporter
{
    public class Model
    {
        /// <summary>
        /// Root element of the documentation
        /// </summary>
        public DocProject Project { get; private set; }

        private Dictionary<Type, List<SEntity>> _instances;

        // static metadata used to create dictionary of types and their non-abstract children
        private static readonly Dictionary<Type, List<Type>> _metaData = new Dictionary<Type, List<Type>>();

        static Model()
        {
            var types = typeof(SEntity).Assembly.GetTypes().Where(t => typeof(SEntity).IsAssignableFrom(t)
                && !t.IsAbstract && !t.IsInterface);
            foreach (var type in types)
            {
                if (_metaData.TryGetValue(type, out List<Type> values))
                    continue;

                Type node = type;
                while (node != typeof(SEntity).BaseType)
                {
                    if (_metaData.TryGetValue(node, out List<Type> implementations))
                        implementations.Add(type);
                    else
                        _metaData.Add(node, new List<Type> { type });

                    node = node.BaseType;
                }
            }
        }

        /// <summary>
        /// Opens the IFCDOC model for reading
        /// </summary>
        /// <param name="path"></param>
        public void Open(string path)
        {
            Dictionary<long, object> instances;
            using (var streamDoc = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var serializer = new StepSerializer(typeof(DocProject), SchemaDOC.Types);
                Project = (DocProject)serializer.ReadObject(streamDoc, out instances);
            }

            _instances = new Dictionary<Type, List<SEntity>>();
            foreach (var instance in instances.Values.OfType<SEntity>())
            {
                var type = instance.GetType();
                if (_instances.TryGetValue(type, out List<SEntity> entities))
                    entities.Add(instance);
                else
                    _instances.Add(type, new List<SEntity> { instance });
            }
        }

        /// <summary>
        /// Gets all instances of specified type optionally filtered by the predicate
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="predicate">Optional predicate</param>
        /// <returns></returns>
        public IEnumerable<T> Get<T>(Func<T, bool> predicate = null) where T : SEntity
        {
            // get list of non-abstract subtypes
            if (!_metaData.TryGetValue(typeof(T), out List<Type> types))
                yield break;

            // iterate over non-abstract types
            foreach (var type in types)
            {
                if (_instances.TryGetValue(type, out List<SEntity> entities))
                {
                    foreach (var entity in entities.Cast<T>())
                    {
                        // check predicate if defined
                        if (predicate == null || predicate(entity))
                            yield return entity;
                    }
                }
            }
        }
    }
}
