using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
using Unity.Netcode;
#else
using Netcode.io.OLD.UnityEngine;
using Unity = Netcode.io.OLD.Unity;
using UnityEngine = Netcode.io.OLD.UnityEngine;
#endif

namespace Netcode.io.Editor
{
    public static class NetcodeSerializer
    {
        private static readonly JsonSerializerSettings Settings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Auto,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            FloatFormatHandling = FloatFormatHandling.DefaultValue,
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter(),
                new UnityObjectConverter()
            }
        };
        
        public static string Serialize(object obj) => JsonConvert.SerializeObject(obj, Formatting.Indented, Settings);

        public static T Deserialize<T>(string str, T existingObject = null) where T : class
        {
            return JsonConvert.DeserializeObject<T>(str, Settings);
        }

        private class UnityObjectConverter : JsonConverter
        {
            private const string gameObjectProperty = "__go";
            private const string componentsProperty = "__c";
            private const string targetComponentIndex = "__tci";
            
            public override bool CanConvert(Type objectType) => typeof(UnityEngine.Object).IsAssignableFrom(objectType);

            // write is working in Unity Editor flow only
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
#if UNITY_EDITOR
                if (value == null)
                {
                    writer.WriteNull();
                    return;
                }

                switch (value)
                {
                    case ScriptableObject scriptableObject: WriteScriptableObject(scriptableObject); break;
                    case GameObject gameObject: WriteGameObject(gameObject); return;
                    case Transform component: WriteGameObject(component.gameObject, component); break;
                    case NetworkObject component: WriteGameObject(component.gameObject, component); break;
                    case NetworkBehaviour component: WriteGameObject(component.gameObject, component); break;
                    default: writer.WriteNull(); break;
                }

                void WriteScriptableObject(ScriptableObject scriptableObject)
                {
                    writer.WriteRaw(UnityEditor.EditorJsonUtility.ToJson(scriptableObject));
                }

                void WriteGameObject(GameObject gameObject, Component component = null)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(gameObjectProperty);
                    writer.WriteRawValue(UnityEditor.EditorJsonUtility.ToJson(gameObject, true));

                    var components = new List<Component> { gameObject.transform };
                    var networkObject = gameObject.GetComponent<NetworkObject>();
                    if (networkObject != null)
                        components.Add(networkObject);
                
                    var networkBehaviours = gameObject.GetComponentsInChildren<NetworkBehaviour>();
                    if (networkBehaviours != null && networkBehaviours.Length > 0)
                        components.AddRange(networkBehaviours);

                    var index = component == null ? -1 : components.IndexOf(component);
                
                    writer.WritePropertyName(componentsProperty);
                    writer.WriteStartArray();
                    foreach (var c in components)
                        writer.WriteRawValue(UnityEditor.EditorJsonUtility.ToJson(c, true));
                    writer.WriteEndArray();
                
                    writer.WritePropertyName(targetComponentIndex);
                    writer.WriteValue(index);
                
                    writer.WriteEndObject();
                }
#else 
                writer.WriteNull();
#endif
            }

            // read is working in standalone flow only
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                GameObject gameObject;
                Component component;
                if (existingValue is Component existingComponent)
                {
                    component = existingComponent;
                    gameObject = existingComponent.gameObject;
                } else if (existingValue is GameObject existingGameObject)
                {
                    gameObject = existingGameObject;
                    component = null;
                }
                else if (existingValue == null)
                {
                    gameObject = new GameObject();
                    component = null;
                }
                else throw new Exception();

                reader.Read(); // start object token
                serializer.Populate(reader, gameObject);
                reader.Read(); // end object token
                
                return null;
            }
        }
    }
}