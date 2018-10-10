// 4D extension of Pcx by Hiroyuki Inou:
// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pcx
{
    [ScriptedImporter(1, "ply")]
    class PlyImporter : ScriptedImporter
    {
        #region ScriptedImporter implementation

        public enum ContainerType { Mesh, ComputeBuffer }

        [SerializeField] ContainerType _containerType;

        public override void OnImportAsset(AssetImportContext context)
        {
            if (_containerType == ContainerType.Mesh)
            {
                // Mesh container
                // Create a prefab with MeshFilter/MeshRenderer.
                var gameObject = new GameObject();
                bool is4D;
                var mesh = ImportAsMesh(context.assetPath, out is4D);

                var meshFilter = gameObject.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = mesh;

                var meshRenderer = gameObject.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = GetDefaultMaterial(is4D);

                context.AddObjectToAsset("prefab", gameObject);
                if (mesh != null) context.AddObjectToAsset("mesh", mesh);

                context.SetMainObject(gameObject);
            }
            else
            {
                // ComputeBuffer container
                // Create a prefab with PointCloudRenderer.
                var gameObject = new GameObject();
                try
                {
                    var stream = File.Open(context.assetPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    var header = ReadDataHeader(new StreamReader(stream));
                    if (header.is4D) {
                        var data = ImportAsPointCloudData4D(context.assetPath, header, stream);
                        var renderer = gameObject.AddComponent<PointCloud4DRenderer>();
                        renderer.sourceData = data;

                        context.AddObjectToAsset("prefab", gameObject);
                        if (data != null) context.AddObjectToAsset("data", data);
                    } else {
                        var data = ImportAsPointCloudData(context.assetPath, header, stream);

                        var renderer = gameObject.AddComponent<PointCloudRenderer>();
                        renderer.sourceData = data;

                        context.AddObjectToAsset("prefab", gameObject);
                        if (data != null) context.AddObjectToAsset("data", data);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed importing " + context.assetPath + ". " + e.Message);
                }
                context.SetMainObject(gameObject);
            }
        }

        #endregion

        #region Internal utilities

        static Material GetDefaultMaterial(bool is4D)
        {
            if (is4D)
            {
                return AssetDatabase.LoadAssetAtPath<Material>(
                    "Assets/Pcx/Editor/Default Point 4D.mat"
                );
            }
            else
            {
                return AssetDatabase.LoadAssetAtPath<Material>(
                    "Assets/Pcx/Editor/Default Point.mat"
                );
            }
        }

        #endregion

        #region Internal data structure

        enum DataProperty
        {
            Invalid,
            X, Y, Z, W,
            R, G, B, A,
            Data8, Data16, Data32
        }

        static int GetPropertySize(DataProperty p)
        {
            switch (p)
            {
                case DataProperty.X: return 4;
                case DataProperty.Y: return 4;
                case DataProperty.Z: return 4;
                case DataProperty.W: return 4;
                case DataProperty.R: return 1;
                case DataProperty.G: return 1;
                case DataProperty.B: return 1;
                case DataProperty.A: return 1;
                case DataProperty.Data8: return 1;
                case DataProperty.Data16: return 2;
                case DataProperty.Data32: return 4;
            }
            return 0;
        }

        class DataHeader
        {
            public List<DataProperty> properties = new List<DataProperty>();
            public int vertexCount = -1;
            public bool is4D = false;
            public bool isAscii = false;
        }

        class DataBody
        {
            public List<Vector3> vertices;
            public List<Color32> colors;

            public DataBody(int vertexCount)
            {
                vertices = new List<Vector3>(vertexCount);
                colors = new List<Color32>(vertexCount);
            }

            public void AddPoint(
                float x, float y, float z,
                byte r, byte g, byte b, byte a
            )
            {
                vertices.Add(new Vector3(x, y, z));
                colors.Add(new Color32(r, g, b, a));
            }
        }

        class Data4DBody
        {
            public List<Vector4> vertices;
            public List<Color32> colors;

            public Data4DBody(int vertexCount)
            {
                vertices = new List<Vector4>(vertexCount);
                colors = new List<Color32>(vertexCount);
            }

            public void AddPoint(
                float x, float y, float z, float w,
                byte r, byte g, byte b, byte a
            )
            {
                vertices.Add(new Vector4(x, y, z, w));
                colors.Add(new Color32(r, g, b, a));
            }
        }
        #endregion

        #region Reader implementation

        Mesh ImportAsMesh(string path, out bool is4D)
        {
            is4D = false;
            try
            {
                var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var header = ReadDataHeader(new StreamReader(stream));

                var mesh = new Mesh();
                mesh.name = Path.GetFileNameWithoutExtension(path);

                mesh.indexFormat = header.vertexCount > 65535 ?
                    IndexFormat.UInt32 : IndexFormat.UInt16;
                is4D = header.is4D;
                if (header.is4D)
                {
                    var body = header.isAscii ? ReadData4DBodyFromAscii(header, new StreamReader(stream)) : ReadData4DBody(header, new BinaryReader(stream));
                    List<Vector3> vs3 = new List<Vector3>(header.vertexCount);
                    List<Vector2> uvs = new List<Vector2>(header.vertexCount);
                    foreach (var vertex in body.vertices) {
                        vs3.Add((Vector3)vertex);
                        uvs.Add(new Vector2(vertex.w, 0));
                    }
                    mesh.SetVertices(vs3);
                    mesh.SetUVs(1, uvs);
                    mesh.SetColors(body.colors);
                }
                else
                {
                    var body = header.isAscii ? ReadDataBodyFromAscii(header, new StreamReader(stream)) : ReadDataBody(header, new BinaryReader(stream));
                    mesh.SetVertices(body.vertices);
                    mesh.SetColors(body.colors);
                }

                mesh.SetIndices(
                    Enumerable.Range(0, header.vertexCount).ToArray(),
                    MeshTopology.Points, 0
                );

                mesh.UploadMeshData(true);
                return mesh;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed importing " + path + ". " + e.Message);
                return null;
            }
        }

        PointCloudData ImportAsPointCloudData(string path, DataHeader header, Stream stream)
        {
            var body = ReadDataBody(header, new BinaryReader(stream));
            var data = ScriptableObject.CreateInstance<PointCloudData>();
            data.Initialize(body.vertices, body.colors);
            data.name = Path.GetFileNameWithoutExtension(path);
            return data;
        }

        PointCloudData4D ImportAsPointCloudData4D(string path, DataHeader header, Stream stream)
        {
            var body = header.isAscii ? ReadData4DBodyFromAscii(header, new StreamReader(stream)) : ReadData4DBody(header, new BinaryReader(stream));
            var data = ScriptableObject.CreateInstance<PointCloudData4D>();
            data.Initialize(body.vertices, body.colors);
            data.name = Path.GetFileNameWithoutExtension(path);
            return data;
        }

        DataHeader ReadDataHeader(StreamReader reader)
        {
            var data = new DataHeader();
            var readCount = 0;

            // Magic number line ("ply")
            var line = reader.ReadLine();
            readCount += line.Length + 1;
            if (line != "ply")
                throw new ArgumentException("Magic number ('ply') mismatch.");

            // Data format: check if it's binary/little endian.
            line = reader.ReadLine();
            readCount += line.Length + 1;
            if (line == "format ascii 1.0") {
                data.isAscii = true;
            } else if (line != "format binary_little_endian 1.0")
                throw new ArgumentException(
                    "Invalid data format ('" + line + "'). " +
                    "Should be binary/little endian.");

            // Read header contents.
            for (var skip = false; ;)
            {
                // Read a line and split it with white space.
                line = reader.ReadLine();
                readCount += line.Length + 1;
                if (line == "end_header") break;
                var col = line.Split();

                // Element declaration (unskippable)
                if (col[0] == "element")
                {
                    if (col[1] == "vertex")
                    {
                        data.vertexCount = Convert.ToInt32(col[2]);
                        skip = false;
                    }
                    else
                    {
                        // Don't read elements other than vertices.
                        skip = true;
                    }
                }

                if (skip) continue;

                // Property declaration line
                if (col[0] == "property")
                {
                    var prop = DataProperty.Invalid;

                    // Parse the property name entry.
                    switch (col[2])
                    {
                        case "x": prop = DataProperty.X; break;
                        case "y": prop = DataProperty.Y; break;
                        case "z": prop = DataProperty.Z; break;
                        case "w": prop = DataProperty.W;
                            data.is4D = true; break;
                        case "red": prop = DataProperty.R; break;
                        case "green": prop = DataProperty.G; break;
                        case "blue": prop = DataProperty.B; break;
                        case "alpha": prop = DataProperty.A; break;
                    }

                    // Check the property type.
                    if (col[1] == "char" || col[1] == "uchar" || col[1] == "int8" || col[1] == "uint8")
                    {
                        if (prop == DataProperty.Invalid)
                            prop = DataProperty.Data8;
                        else if (GetPropertySize(prop) != 1)
                            throw new ArgumentException("Invalid property type ('" + line + "').");
                    }
                    else if (col[1] == "short" || col[1] == "ushort" || col[1] == "int16" || col[1] == "uint16")
                    {
                        if (prop == DataProperty.Invalid)
                            prop = DataProperty.Data16;
                        else if (GetPropertySize(prop) != 2)
                            throw new ArgumentException("Invalid property type ('" + line + "').");
                    }
                    else if (col[1] == "int" || col[1] == "uint" || col[1] == "float" || 
                             col[1] == "int32" || col[1] == "uint32" || col[1] == "float32")
                    {
                        if (prop == DataProperty.Invalid)
                            prop = DataProperty.Data32;
                        else if (GetPropertySize(prop) != 4)
                            throw new ArgumentException("Invalid property type ('" + line + "').");
                    }
                    else
                    {
                        throw new ArgumentException("Unsupported property type ('" + line + "').");
                    }

                    data.properties.Add(prop);
                }
            }

            // Rewind the stream back to the exact position of the reader.
            reader.BaseStream.Position = readCount;

            return data;
        }

        DataBody ReadDataBody(DataHeader header, BinaryReader reader)
        {
            var data = new DataBody(header.vertexCount);

            float x = 0, y = 0, z = 0;
            Byte r = 255, g = 255, b = 255, a = 255;

            for (var i = 0; i < header.vertexCount; i++)
            {
                foreach (var prop in header.properties)
                {
                    switch (prop)
                    {
                        case DataProperty.X: x = reader.ReadSingle(); break;
                        case DataProperty.Y: y = reader.ReadSingle(); break;
                        case DataProperty.Z: z = reader.ReadSingle(); break;
                        case DataProperty.W: reader.ReadSingle(); break;

                        case DataProperty.R: r = reader.ReadByte(); break;
                        case DataProperty.G: g = reader.ReadByte(); break;
                        case DataProperty.B: b = reader.ReadByte(); break;
                        case DataProperty.A: a = reader.ReadByte(); break;

                        case DataProperty.Data8: reader.ReadByte(); break;
                        case DataProperty.Data16: reader.BaseStream.Position += 2; break;
                        case DataProperty.Data32: reader.BaseStream.Position += 4; break;
                    }
                }

                data.AddPoint(x, y, z, r, g, b, a);
            }

            return data;
        }

        Data4DBody ReadData4DBody(DataHeader header, BinaryReader reader)
        {
            var data = new Data4DBody(header.vertexCount);

            float x = 0, y = 0, z = 0, w = 0;
            Byte r = 255, g = 255, b = 255, a = 255;

            for (var i = 0; i < header.vertexCount; i++)
            {
                foreach (var prop in header.properties)
                {
                    switch (prop)
                    {
                        case DataProperty.X: x = reader.ReadSingle(); break;
                        case DataProperty.Y: y = reader.ReadSingle(); break;
                        case DataProperty.Z: z = reader.ReadSingle(); break;
                        case DataProperty.W: w = reader.ReadSingle(); break;

                        case DataProperty.R: r = reader.ReadByte(); break;
                        case DataProperty.G: g = reader.ReadByte(); break;
                        case DataProperty.B: b = reader.ReadByte(); break;
                        case DataProperty.A: a = reader.ReadByte(); break;

                        case DataProperty.Data8: reader.ReadByte(); break;
                        case DataProperty.Data16: reader.BaseStream.Position += 2; break;
                        case DataProperty.Data32: reader.BaseStream.Position += 4; break;
                    }
                }

                data.AddPoint(x, y, z, w, r, g, b, a);
            }

            return data;
        }

        DataBody ReadDataBodyFromAscii(DataHeader header, StreamReader reader)
        {
            var data = new DataBody(header.vertexCount);

            float x = 0, y = 0, z = 0;
            Byte r = 255, g = 255, b = 255, a = 255;

            for (var i = 0; i < header.vertexCount; i++)
            {
                var line = reader.ReadLine();
                var col = line.Split();
                int j = 0;
                foreach (var prop in header.properties)
                {
                    switch (prop)
                    {
                        case DataProperty.X: x = float.Parse(col[j]); break;
                        case DataProperty.Y: y = float.Parse(col[j]); break;
                        case DataProperty.Z: z = float.Parse(col[j]); break;
                        case DataProperty.W: break;

                        case DataProperty.R: r = byte.Parse(col[j]); break;
                        case DataProperty.G: g = byte.Parse(col[j]); break;
                        case DataProperty.B: b = byte.Parse(col[j]); break;
                        case DataProperty.A: a = byte.Parse(col[j]); break;
                            /*
                            case DataProperty.Data8: reader.ReadByte(); break;
                            case DataProperty.Data16: reader.BaseStream.Position += 2; break;
                            case DataProperty.Data32: reader.BaseStream.Position += 4; break;
                            */
                    }
                    j++;
                }

                data.AddPoint(x, y, z, r, g, b, a);
            }

            return data;
        }

        Data4DBody ReadData4DBodyFromAscii(DataHeader header, StreamReader reader)
        {
            var data = new Data4DBody(header.vertexCount);

            float x = 0, y = 0, z = 0, w = 0;
            Byte r = 255, g = 255, b = 255, a = 255;

            for (var i = 0; i < header.vertexCount; i++)
            {
                var line = reader.ReadLine();
                var col = line.Split();
                int j = 0;
                foreach (var prop in header.properties)
                {
                    switch (prop)
                    {
                        case DataProperty.X: x = float.Parse(col[j]); break;
                        case DataProperty.Y: y = float.Parse(col[j]); break;
                        case DataProperty.Z: z = float.Parse(col[j]); break;
                        case DataProperty.W: w = float.Parse(col[j]); break;

                        case DataProperty.R: r = byte.Parse(col[j]); break;
                        case DataProperty.G: g = byte.Parse(col[j]); break;
                        case DataProperty.B: b = byte.Parse(col[j]); break;
                        case DataProperty.A: a = byte.Parse(col[j]); break;
                        /*
                        case DataProperty.Data8: reader.ReadByte(); break;
                        case DataProperty.Data16: reader.BaseStream.Position += 2; break;
                        case DataProperty.Data32: reader.BaseStream.Position += 4; break;
                        */
                    }
                    j++;
                }

                data.AddPoint(x, y, z, w, r, g, b, a);
            }

            return data;
        }

    }

    #endregion
}
