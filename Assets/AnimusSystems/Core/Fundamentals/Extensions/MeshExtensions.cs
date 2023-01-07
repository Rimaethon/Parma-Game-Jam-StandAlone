using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class MeshExtensions  {
    class Vertex
    {
        public int IDX;
        public int GroupID;
        public Vector3 Coord;
        public Vector2 UVCoord;
        public BoneWeight boneWeight;
        public Vector3 Normal;
    }
    class Triangle
    {
        public Vertex first;
        public Vertex second;
        public Vertex third;
        public int SubMeshIndex;
    }
    public static Mesh Copy(this Mesh mesh)
    {
        return GameObject.Instantiate(mesh);
    }
    public async static Task<Mesh> SimpleSplit(this Mesh mesh, bool[] mask)
    {
        var SeparatedMesh = mesh.Copy();

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            List<ushort> meshTris = new List<ushort>();
            mesh.GetTriangles(meshTris, i);
            List<ushort> separationTris = new List<ushort>();
            await Task.Run(delegate
            {
                for (int j = 0; j < meshTris.Count; j += 3)
                {
                    if (mask[meshTris[j]] && mask[meshTris[j+1]] && mask[meshTris[j + 2]])
                    {
                        separationTris.Add(meshTris[j]);
                        separationTris.Add(meshTris[j + 1]);
                        separationTris.Add(meshTris[j + 2]);

                        meshTris.RemoveRange(j, 3);
                        j -= 3;
                    }
                }
            });
            SeparatedMesh.SetTriangles(separationTris, i);
            mesh.SetTriangles(meshTris, i);
        }
        return SeparatedMesh;
    }
    public static Mesh SeparateVerts(this Mesh mesh, params int[] indices)
    {
        //1. Get mesh data
        var Verts = mesh.vertices;
        var UVs = mesh.uv;
        var Tris = new int[mesh.subMeshCount][];
        for (int i = 0; i < mesh.subMeshCount; i++) Tris[i] = mesh.GetTriangles(i);
        var submeshes = mesh.subMeshCount;
        var Bindposes = mesh.bindposes;
        var BoneWeights = mesh.boneWeights;
        var Normals = mesh.normals;

        //2. Aggregate all data in convenient structures
        var Vertices = new Vertex[Verts.Length];
        var MeshTris = new Triangle[mesh.triangles.Length/3];
        for (int i = 0; i < Verts.Length; i++)
        {
            Vertices[i] = new Vertex() {
                GroupID = 0,
                Coord = Verts[i],
                boneWeight = BoneWeights[i],
                UVCoord = UVs[i],
                Normal = Normals[i]
            };
        }
        int triangleCounter = 0;
        for (int submesh = 0; submesh < Tris.Length; submesh++)
        {
            for (int i = 0; i < Tris[submesh].Length; i += 3)
            {
                MeshTris[triangleCounter] = new Triangle() {
                    first = Vertices[Tris[submesh][i]],
                    second = Vertices[Tris[submesh][i + 1]],
                    third = Vertices[Tris[submesh][i + 2]],
                    SubMeshIndex = submesh
                };
                triangleCounter++;
            }
        }
        //3. Separate triangles by GroupID
        for (int i = 0; i < indices.Length; i++) Vertices[indices[i]].GroupID = 1;
        
        var Separation =  System.Array.FindAll(MeshTris, t => t.first.GroupID == 1 && t.second.GroupID == 1 && t.third.GroupID == 1);
        MeshTris = System.Array.FindAll(MeshTris, t => t.first.GroupID == 0 && t.second.GroupID == 0 && t.third.GroupID == 0);

        //4. Convert list of vertices back to arrays
        var MeshVerts = new Vector3[Vertices.Length - indices.Length];
        var MeshUVs = new Vector2[Vertices.Length - indices.Length];
        var MeshBoneWeights = new BoneWeight[Vertices.Length - indices.Length];
        var MeshNormals = new Vector3[Vertices.Length - indices.Length];
        int MeshIdx = 0;

        var SeparationVerts = new Vector3[indices.Length];
        var SeparationUVs = new Vector2[indices.Length];
        var SeparationBoneWeights = new BoneWeight[indices.Length];
        var SeparationNormals = new Vector3[indices.Length];
        int SeparationIdx = 0;

        for (int i=0; i<Vertices.Length; i++)
        {
            if (Vertices[i].GroupID==0)
            {
                MeshVerts[MeshIdx] = Vertices[i].Coord;
                MeshUVs[MeshIdx] = Vertices[i].UVCoord;
                MeshBoneWeights[MeshIdx] = Vertices[i].boneWeight;
                MeshNormals[MeshIdx] = Vertices[i].Normal;
                Vertices[i].IDX = MeshIdx;
                MeshIdx++;
            } else
            {
                SeparationVerts[SeparationIdx] = Vertices[i].Coord;
                SeparationUVs[SeparationIdx] = Vertices[i].UVCoord;
                SeparationBoneWeights[SeparationIdx] = Vertices[i].boneWeight;
                SeparationNormals[SeparationIdx] = Vertices[i].Normal;
                Vertices[i].IDX = SeparationIdx;
                SeparationIdx++;
            }
        }

        //5. Update vertex data of mesh
        mesh.Clear();
        mesh.vertices = MeshVerts;
        mesh.uv = MeshUVs;
        mesh.bindposes = Bindposes;
        mesh.boneWeights = MeshBoneWeights;
        mesh.normals = MeshNormals;

        //6. Split all triangles by submesh index
        var SubmeshedTris = new List<int>[submeshes];
        for (int i = 0; i < submeshes; i++) SubmeshedTris[i] = new List<int>();
        for (int i = 0; i < MeshTris.Length; i++)
        {
            SubmeshedTris[MeshTris[i].SubMeshIndex].Add(MeshTris[i].first.IDX);
            SubmeshedTris[MeshTris[i].SubMeshIndex].Add(MeshTris[i].second.IDX);
            SubmeshedTris[MeshTris[i].SubMeshIndex].Add(MeshTris[i].third.IDX);
        }

        //7. Update triangles and submeshes count for mesh
        mesh.subMeshCount = submeshes;
        for (int i=0; i<submeshes;i++)
        {
            mesh.SetTriangles(SubmeshedTris[i],i);
        }

        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        //8. Set vertex data for separated mesh
        var SeparatedMesh = new Mesh();
        SeparatedMesh.vertices = SeparationVerts;
        SeparatedMesh.uv = SeparationUVs;
        SeparatedMesh.bindposes = Bindposes;
        SeparatedMesh.boneWeights = SeparationBoneWeights;
        SeparatedMesh.normals = SeparationNormals;

        //9. Split all triangles by submesh index
        SubmeshedTris = new List<int>[submeshes];
        for (int i = 0; i < submeshes; i++) SubmeshedTris[i] = new List<int>();
        for (int i = 0; i < Separation.Length; i++)
        {
            SubmeshedTris[Separation[i].SubMeshIndex].Add(Separation[i].first.IDX);
            SubmeshedTris[Separation[i].SubMeshIndex].Add(Separation[i].second.IDX);
            SubmeshedTris[Separation[i].SubMeshIndex].Add(Separation[i].third.IDX);
        }

        //10. Update triangles and submeshes count for separated mesh
        SeparatedMesh.subMeshCount = submeshes;
        for (int i = 0; i < submeshes; i++)
        {
            SeparatedMesh.SetTriangles(SubmeshedTris[i], i);
        }
        
        SeparatedMesh.RecalculateBounds();
        SeparatedMesh.RecalculateTangents();


        return SeparatedMesh;
    }
    public static SkinnedMeshRenderer Copy(this SkinnedMeshRenderer skinnedMesh, GameObject Destination)
    {
        var MeshObject = new GameObject(skinnedMesh.name);
        MeshObject.transform.SetParent(Destination.transform);
        var Copy = MeshObject.AddComponent<SkinnedMeshRenderer>();
        
        Copy.sharedMaterial = skinnedMesh.sharedMaterial;
        Copy.sharedMaterials = skinnedMesh.sharedMaterials;
        Copy.sharedMesh = skinnedMesh.sharedMesh;
        Copy.bones = skinnedMesh.bones;
        Copy.rootBone = skinnedMesh.rootBone;
        return Copy;
    }
}
