xof 0303txt 0032

// Generated by 3D Rad Exporter plugin for Google SketchUp - http://www.3drad.com

template Header {
<3D82AB43-62DA-11cf-AB39-0020AF71E433>
WORD major;
WORD minor;
DWORD flags;
}
template Vector {
<3D82AB5E-62DA-11cf-AB39-0020AF71E433>
FLOAT x;
FLOAT y;
FLOAT z;
}
template Coords2d {
<F6F23F44-7686-11cf-8F52-0040333594A3>
FLOAT u;
FLOAT v;
}
template Matrix4x4 {
<F6F23F45-7686-11cf-8F52-0040333594A3>
array FLOAT matrix[16];
}
template ColorRGBA {
<35FF44E0-6C7C-11cf-8F52-0040333594A3>
FLOAT red;
FLOAT green;
FLOAT blue;
FLOAT alpha;
}
template ColorRGB {
<D3E16E81-7835-11cf-8F52-0040333594A3>
FLOAT red;
FLOAT green;
FLOAT blue;
}
template IndexedColor {
<1630B820-7842-11cf-8F52-0040333594A3>
DWORD index;
ColorRGBA indexColor;
}
template Boolean {
<4885AE61-78E8-11cf-8F52-0040333594A3>
WORD truefalse;
}
template Boolean2d {
<4885AE63-78E8-11cf-8F52-0040333594A3>
Boolean u;
Boolean v;
}
template MaterialWrap {
<4885AE60-78E8-11cf-8F52-0040333594A3>
Boolean u;
Boolean v;
}
template TextureFilename {
<A42790E1-7810-11cf-8F52-0040333594A3>
STRING filename;
}
template Material {
<3D82AB4D-62DA-11cf-AB39-0020AF71E433>
ColorRGBA faceColor;
FLOAT power;
ColorRGB specularColor;
ColorRGB emissiveColor;
[...]
}
template MeshFace {
<3D82AB5F-62DA-11cf-AB39-0020AF71E433>
DWORD nFaceVertexIndices;
array DWORD faceVertexIndices[nFaceVertexIndices];
}
template MeshFaceWraps {
<4885AE62-78E8-11cf-8F52-0040333594A3>
DWORD nFaceWrapValues;
Boolean2d faceWrapValues;
}
template MeshTextureCoords {
<F6F23F40-7686-11cf-8F52-0040333594A3>
DWORD nTextureCoords;
array Coords2d textureCoords[nTextureCoords];
}
template MeshMaterialList {
<F6F23F42-7686-11cf-8F52-0040333594A3>
DWORD nMaterials;
DWORD nFaceIndexes;
array DWORD faceIndexes[nFaceIndexes];
[Material]
}
template MeshNormals {
<F6F23F43-7686-11cf-8F52-0040333594A3>
DWORD nNormals;
array Vector normals[nNormals];
DWORD nFaceNormals;
array MeshFace faceNormals[nFaceNormals];
}
template MeshVertexColors {
<1630B821-7842-11cf-8F52-0040333594A3>
DWORD nVertexColors;
array IndexedColor vertexColors[nVertexColors];
}
template Mesh {
<3D82AB44-62DA-11cf-AB39-0020AF71E433>
DWORD nVertices;
array Vector vertices[nVertices];
DWORD nFaces;
array MeshFace faces[nFaces];
[...]
}
template FrameTransformMatrix {
<F6F23F41-7686-11cf-8F52-0040333594A3>
Matrix4x4 frameMatrix;
}
template Frame {
<3D82AB46-62DA-11cf-AB39-0020AF71E433>
[...]
}
template XSkinMeshHeader {
<3cf169ce-ff7c-44ab-93c0-f78f62d172e2>
WORD nMaxSkinWeightsPerVertex;
WORD nMaxSkinWeightsPerFace;
WORD nBones;
}
template VertexDuplicationIndices {
<b8d65549-d7c9-4995-89cf-53a9a8b031e3>
DWORD nIndices;
DWORD nOriginalVertices;
array DWORD indices[nIndices];
}
template SkinWeights {
<6f0d123b-bad2-4167-a0d0-80224f25fabb>
STRING transformNodeName;
DWORD nWeights;
array DWORD vertexIndices[nWeights];
array FLOAT weights[nWeights];
Matrix4x4 matrixOffset;
}
Frame RAD_SCENE_ROOT {
FrameTransformMatrix {
1.000000,0.000000,0.000000,0.000000,0.000000,1.000000,0.000000,0.000000,0.000000,0.000000,1.000000,0.000000,0.000000,0.000000,0.000000,1.000000;;
}
Frame RAD_FRAME {
FrameTransformMatrix {
1.000000,0.000000,0.000000,0.000000,0.000000,1.000000,0.000000,0.000000,0.000000,0.000000,1.000000,0.000000,0.000000,0.000000,0.000000,1.000000;;
}
Mesh RAD_MESH {
48;
26.304121;27.275570;0.000000;,
23.166583;-0.024416;0.000000;,
26.304121;-0.024416;0.000000;,
23.166583;6.125338;0.000000;,
2.055003;6.125338;0.000000;,
-0.005865;-0.024416;0.000000;,
2.055003;-0.024416;0.000000;,
-0.005865;27.275570;0.000000;,
26.304121;-0.024416;7.649996;,
26.304121;27.275570;0.000000;,
26.304121;-0.024416;0.000000;,
26.304121;27.275570;7.649996;,
26.304121;-0.024416;7.649996;,
23.166583;-0.024416;0.000000;,
23.166583;-0.024416;7.649996;,
26.304121;-0.024416;0.000000;,
2.055003;-0.024416;7.649996;,
-0.005865;-0.024416;0.000000;,
-0.005865;-0.024416;7.649996;,
2.055003;-0.024416;0.000000;,
23.166583;6.125338;7.649996;,
2.055003;6.125338;0.000000;,
2.055003;6.125338;7.649996;,
23.166583;6.125338;0.000000;,
-0.005865;27.275570;7.649996;,
2.055003;-0.024416;7.649996;,
-0.005865;-0.024416;7.649996;,
2.055003;6.125338;7.649996;,
23.166583;6.125338;7.649996;,
26.304121;-0.024416;7.649996;,
23.166583;-0.024416;7.649996;,
26.304121;27.275570;7.649996;,
23.166583;6.125338;7.649996;,
23.166583;-0.024416;0.000000;,
23.166583;6.125338;0.000000;,
23.166583;-0.024416;7.649996;,
-0.005865;27.275570;7.649996;,
-0.005865;-0.024416;0.000000;,
-0.005865;27.275570;0.000000;,
-0.005865;-0.024416;7.649996;,
2.055003;-0.024416;7.649996;,
2.055003;6.125338;0.000000;,
2.055003;-0.024416;0.000000;,
2.055003;6.125338;7.649996;,
26.304121;27.275570;0.000000;,
-0.005865;27.275570;7.649996;,
-0.005865;27.275570;0.000000;,
26.304121;27.275570;7.649996;;
28;
3;2,1,0,
3;3,0,1,
3;4,0,3,
3;6,5,4,
3;7,4,5,
3;0,4,7,
3;10,9,8,
3;11,8,9,
3;14,13,12,
3;15,12,13,
3;18,17,16,
3;19,16,17,
3;22,21,20,
3;23,20,21,
3;26,25,24,
3;27,24,25,
3;28,24,27,
3;30,29,28,
3;31,28,29,
3;24,28,31,
3;34,33,32,
3;35,32,33,
3;38,37,36,
3;39,36,37,
3;42,41,40,
3;43,40,41,
3;46,45,44,
3;47,44,45;;
MeshNormals {
48;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
1.000000;0.000000;0.000000;
1.000000;0.000000;0.000000;
1.000000;0.000000;0.000000;
1.000000;0.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;0.000000;1.000000;
0.000000;0.000000;1.000000;
0.000000;0.000000;1.000000;
0.000000;0.000000;1.000000;
0.000000;0.000000;1.000000;
0.000000;0.000000;1.000000;
0.000000;0.000000;1.000000;
0.000000;0.000000;1.000000;
-1.000000;0.000000;0.000000;
-1.000000;0.000000;0.000000;
-1.000000;0.000000;0.000000;
-1.000000;0.000000;0.000000;
-1.000000;0.000000;0.000000;
-1.000000;0.000000;0.000000;
-1.000000;0.000000;0.000000;
-1.000000;0.000000;0.000000;
1.000000;0.000000;0.000000;
1.000000;0.000000;0.000000;
1.000000;0.000000;0.000000;
1.000000;0.000000;0.000000;
0.000000;1.000000;0.000000;
0.000000;1.000000;0.000000;
0.000000;1.000000;0.000000;
0.000000;1.000000;0.000000;;
28;
3;2,1,0;
3;3,0,1;
3;4,0,3;
3;6,5,4;
3;7,4,5;
3;0,4,7;
3;10,9,8;
3;11,8,9;
3;14,13,12;
3;15,12,13;
3;18,17,16;
3;19,16,17;
3;22,21,20;
3;23,20,21;
3;26,25,24;
3;27,24,25;
3;28,24,27;
3;30,29,28;
3;31,28,29;
3;24,28,31;
3;34,33,32;
3;35,32,33;
3;38,37,36;
3;39,36,37;
3;42,41,40;
3;43,40,41;
3;46,45,44;
3;47,44,45;;
}
MeshTextureCoords {
48;
1036.595864,-1073.841903;
913.070685,0.961247;
1036.595864,0.961247;
913.070685,-241.155182;
81.905662,-241.155182;
0.769092,0.961247;
81.905662,0.961247;
0.769092,-1073.841903;
302.181102,0.961247;
1.000000,-1073.841903;
1.000000,0.961247;
302.181102,-1073.841903;
-1034.595864,-301.181102;
-911.070685,-0.000000;
-911.070685,-301.181102;
-1034.595864,-0.000000;
-79.905662,-301.181102;
1.230908,-0.000000;
1.230908,-301.181102;
-79.905662,-0.000000;
-911.070685,-301.181102;
-79.905662,-0.000000;
-79.905662,-301.181102;
-911.070685,-0.000000;
1.230908,-1073.841903;
-79.905662,0.961247;
1.230908,0.961247;
-79.905662,-241.155182;
-911.070685,-241.155182;
-1034.595864,0.961247;
-911.070685,0.961247;
-1034.595864,-1073.841903;
-300.181102,-241.155182;
1.000000,0.961247;
1.000000,-241.155182;
-300.181102,0.961247;
-300.181102,-1073.841903;
1.000000,0.961247;
1.000000,-1073.841903;
-300.181102,0.961247;
302.181102,0.961247;
1.000000,-241.155182;
1.000000,0.961247;
302.181102,-241.155182;
1036.595864,-0.000000;
0.769092,-301.181102;
0.769092,-0.000000;
1036.595864,-301.181102;;
}
MeshMaterialList {
1;
28;
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0;
Material {
1.000000;1.000000;1.000000;1.000000;;
50.000000;
0.000000;0.000000;0.000000;;
0.000000;0.000000;0.000000;;
}
}
}
}
}