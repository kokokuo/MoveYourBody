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
276;
10.768320;8.933331;7.649996;,
10.768320;14.373328;0.000000;,
10.768320;8.933331;0.000000;,
10.768320;14.373328;7.649996;,
16.208317;14.373328;7.649996;,
16.208317;8.933331;0.000000;,
16.208317;14.373328;0.000000;,
16.208317;8.933331;7.649996;,
9.739595;24.166423;7.649996;,
10.360967;24.976210;0.000000;,
9.739595;24.166423;0.000000;,
10.360967;24.976210;7.649996;,
9.215755;22.211424;7.649996;,
9.348985;23.223406;0.000000;,
9.215755;22.211424;0.000000;,
9.348985;23.223406;7.649996;,
10.768320;14.373328;0.000000;,
2.360918;14.373328;7.649996;,
2.360918;14.373328;0.000000;,
10.768320;14.373328;7.649996;,
11.170754;25.597581;7.649996;,
10.360967;24.976210;0.000000;,
10.360967;24.976210;7.649996;,
11.170754;25.597581;0.000000;,
12.113771;25.988192;7.649996;,
11.170754;25.597581;0.000000;,
11.170754;25.597581;7.649996;,
12.113771;25.988192;0.000000;,
9.348985;23.223406;7.649996;,
9.739595;24.166423;0.000000;,
9.348985;23.223406;0.000000;,
9.739595;24.166423;7.649996;,
9.348985;21.199442;7.649996;,
9.215755;22.211424;0.000000;,
9.348985;21.199442;0.000000;,
9.215755;22.211424;7.649996;,
9.739595;20.256425;7.649996;,
9.348985;21.199442;0.000000;,
9.739595;20.256425;0.000000;,
9.348985;21.199442;7.649996;,
13.125753;26.121422;7.649996;,
12.113771;25.988192;0.000000;,
12.113771;25.988192;7.649996;,
13.125753;26.121422;0.000000;,
16.902521;21.199442;7.649996;,
16.511910;20.256425;0.000000;,
16.902521;21.199442;0.000000;,
16.511910;20.256425;7.649996;,
16.902521;23.223406;7.649996;,
17.035751;22.211424;0.000000;,
16.902521;23.223406;0.000000;,
17.035751;22.211424;7.649996;,
26.304121;27.275570;0.000000;,
-0.005865;27.275570;7.649996;,
-0.005865;27.275570;0.000000;,
26.304121;27.275570;7.649996;,
14.137735;25.988192;7.649996;,
13.125753;26.121422;0.000000;,
13.125753;26.121422;7.649996;,
14.137735;25.988192;0.000000;,
17.035751;22.211424;7.649996;,
16.902521;21.199442;0.000000;,
17.035751;22.211424;0.000000;,
16.902521;21.199442;7.649996;,
10.360967;19.446638;7.649996;,
9.739595;20.256425;0.000000;,
10.360967;19.446638;0.000000;,
9.739595;20.256425;7.649996;,
15.890539;19.446638;0.000000;,
15.495254;19.143325;7.649996;,
15.495254;19.143325;0.000000;,
15.890539;19.446638;7.649996;,
3.559104;-0.024416;7.649996;,
-0.005865;-0.024416;0.000000;,
-0.005865;-0.024416;7.649996;,
3.559104;-0.024416;0.000000;,
15.080752;25.597581;7.649996;,
14.137735;25.988192;0.000000;,
14.137735;25.988192;7.649996;,
15.080752;25.597581;0.000000;,
13.149128;-0.024416;7.649996;,
14.505880;3.552560;7.649996;,
15.896116;-0.024416;7.649996;,
11.791077;1.549274;7.649996;,
12.838427;2.845795;7.649996;,
24.110907;14.373328;0.000000;,
16.208317;14.373328;7.649996;,
16.208317;14.373328;0.000000;,
24.110907;14.373328;7.649996;,
15.890539;24.976210;7.649996;,
15.080752;25.597581;0.000000;,
15.080752;25.597581;7.649996;,
15.890539;24.976210;0.000000;,
10.756252;19.143325;0.000000;,
10.360967;19.446638;7.649996;,
10.360967;19.446638;0.000000;,
10.756252;19.143325;7.649996;,
24.110907;19.143325;7.649996;,
15.495254;19.143325;0.000000;,
15.495254;19.143325;7.649996;,
24.110907;19.143325;0.000000;,
16.511910;20.256425;7.649996;,
15.890539;19.446638;0.000000;,
16.511910;20.256425;0.000000;,
15.890539;19.446638;7.649996;,
2.360918;14.373328;7.649996;,
2.360918;19.143325;0.000000;,
2.360918;14.373328;0.000000;,
2.360918;19.143325;7.649996;,
15.890539;24.976210;7.649996;,
16.511910;24.166423;0.000000;,
15.890539;24.976210;0.000000;,
16.511910;24.166423;7.649996;,
14.505880;3.552560;0.000000;,
13.149128;-0.024416;0.000000;,
15.896116;-0.024416;0.000000;,
11.791077;1.549274;0.000000;,
12.838427;2.845795;0.000000;,
10.756252;19.143325;7.649996;,
2.360918;19.143325;0.000000;,
2.360918;19.143325;7.649996;,
10.756252;19.143325;0.000000;,
24.110907;19.143325;7.649996;,
24.110907;14.373328;0.000000;,
24.110907;19.143325;0.000000;,
24.110907;14.373328;7.649996;,
-0.005865;27.275570;7.649996;,
-0.005865;-0.024416;0.000000;,
-0.005865;27.275570;0.000000;,
-0.005865;-0.024416;7.649996;,
16.511910;24.166423;7.649996;,
16.902521;23.223406;0.000000;,
16.511910;24.166423;0.000000;,
16.902521;23.223406;7.649996;,
26.304121;-0.024416;7.649996;,
26.304121;27.275570;0.000000;,
26.304121;-0.024416;0.000000;,
26.304121;27.275570;7.649996;,
3.559104;-0.024416;7.649996;,
4.448666;1.989789;0.000000;,
3.559104;-0.024416;0.000000;,
4.448666;1.989789;7.649996;,
4.448666;1.989789;7.649996;,
6.144651;4.155223;0.000000;,
4.448666;1.989789;0.000000;,
6.144651;4.155223;7.649996;,
19.894043;5.477070;7.649996;,
22.159164;4.566514;0.000000;,
19.894043;5.477070;0.000000;,
22.159164;4.566514;7.649996;,
15.896116;-0.024416;7.649996;,
13.149128;-0.024416;0.000000;,
13.149128;-0.024416;7.649996;,
15.896116;-0.024416;0.000000;,
26.304121;-0.024416;7.649996;,
23.384455;-0.024416;0.000000;,
23.384455;-0.024416;7.649996;,
26.304121;-0.024416;0.000000;,
10.360967;19.446638;0.000000;,
2.360918;19.143325;0.000000;,
10.756252;19.143325;0.000000;,
26.304121;27.275570;0.000000;,
23.384455;-0.024416;0.000000;,
26.304121;-0.024416;0.000000;,
23.597473;2.589399;0.000000;,
24.110907;14.373328;0.000000;,
22.159164;4.566514;0.000000;,
19.894043;5.477070;0.000000;,
16.208317;8.933331;0.000000;,
24.110907;19.143325;0.000000;,
15.495254;19.143325;0.000000;,
15.890539;19.446638;0.000000;,
16.511910;20.256425;0.000000;,
16.902521;21.199442;0.000000;,
17.035751;22.211424;0.000000;,
16.902521;23.223406;0.000000;,
16.511910;24.166423;0.000000;,
15.890539;24.976210;0.000000;,
15.080752;25.597581;0.000000;,
14.137735;25.988192;0.000000;,
13.125753;26.121422;0.000000;,
4.448666;1.989789;0.000000;,
-0.005865;-0.024416;0.000000;,
3.559104;-0.024416;0.000000;,
-0.005865;27.275570;0.000000;,
2.360918;14.373328;0.000000;,
6.144651;4.155223;0.000000;,
8.257639;7.161674;0.000000;,
10.768320;8.933331;0.000000;,
10.768320;14.373328;0.000000;,
9.739595;20.256425;0.000000;,
9.348985;21.199442;0.000000;,
9.215755;22.211424;0.000000;,
9.348985;23.223406;0.000000;,
9.739595;24.166423;0.000000;,
10.360967;24.976210;0.000000;,
11.170754;25.597581;0.000000;,
12.113771;25.988192;0.000000;,
16.208317;14.373328;0.000000;,
15.890539;19.446638;7.649996;,
24.110907;19.143325;7.649996;,
15.495254;19.143325;7.649996;,
-0.005865;27.275570;7.649996;,
3.559104;-0.024416;7.649996;,
-0.005865;-0.024416;7.649996;,
4.448666;1.989789;7.649996;,
2.360918;14.373328;7.649996;,
6.144651;4.155223;7.649996;,
8.257639;7.161674;7.649996;,
10.768320;8.933331;7.649996;,
2.360918;19.143325;7.649996;,
10.756252;19.143325;7.649996;,
10.360967;19.446638;7.649996;,
9.739595;20.256425;7.649996;,
9.348985;21.199442;7.649996;,
9.215755;22.211424;7.649996;,
9.348985;23.223406;7.649996;,
9.739595;24.166423;7.649996;,
10.360967;24.976210;7.649996;,
11.170754;25.597581;7.649996;,
12.113771;25.988192;7.649996;,
13.125753;26.121422;7.649996;,
23.597473;2.589399;7.649996;,
26.304121;-0.024416;7.649996;,
23.384455;-0.024416;7.649996;,
26.304121;27.275570;7.649996;,
24.110907;14.373328;7.649996;,
22.159164;4.566514;7.649996;,
19.894043;5.477070;7.649996;,
16.208317;8.933331;7.649996;,
16.208317;14.373328;7.649996;,
16.511910;20.256425;7.649996;,
16.902521;21.199442;7.649996;,
17.035751;22.211424;7.649996;,
16.902521;23.223406;7.649996;,
16.511910;24.166423;7.649996;,
15.890539;24.976210;7.649996;,
15.080752;25.597581;7.649996;,
14.137735;25.988192;7.649996;,
10.768320;14.373328;7.649996;,
6.144651;4.155223;7.649996;,
8.257639;7.161674;0.000000;,
6.144651;4.155223;0.000000;,
8.257639;7.161674;7.649996;,
11.791077;1.549274;7.649996;,
13.149128;-0.024416;0.000000;,
11.791077;1.549274;0.000000;,
13.149128;-0.024416;7.649996;,
12.838427;2.845795;7.649996;,
11.791077;1.549274;0.000000;,
12.838427;2.845795;0.000000;,
11.791077;1.549274;7.649996;,
10.768320;8.933331;7.649996;,
8.257639;7.161674;0.000000;,
8.257639;7.161674;7.649996;,
10.768320;8.933331;0.000000;,
12.838427;2.845795;0.000000;,
14.505880;3.552560;7.649996;,
12.838427;2.845795;7.649996;,
14.505880;3.552560;0.000000;,
15.896116;-0.024416;7.649996;,
14.505880;3.552560;0.000000;,
15.896116;-0.024416;0.000000;,
14.505880;3.552560;7.649996;,
23.597473;2.589399;7.649996;,
23.384455;-0.024416;0.000000;,
23.597473;2.589399;0.000000;,
23.384455;-0.024416;7.649996;,
22.159164;4.566514;7.649996;,
23.597473;2.589399;0.000000;,
22.159164;4.566514;0.000000;,
23.597473;2.589399;7.649996;,
19.894043;5.477070;7.649996;,
16.208317;8.933331;0.000000;,
16.208317;8.933331;7.649996;,
19.894043;5.477070;0.000000;;
176;
3;2,1,0,
3;3,0,1,
3;6,5,4,
3;7,4,5,
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
3;30,29,28,
3;31,28,29,
3;34,33,32,
3;35,32,33,
3;38,37,36,
3;39,36,37,
3;42,41,40,
3;43,40,41,
3;46,45,44,
3;47,44,45,
3;50,49,48,
3;51,48,49,
3;54,53,52,
3;55,52,53,
3;58,57,56,
3;59,56,57,
3;62,61,60,
3;63,60,61,
3;66,65,64,
3;67,64,65,
3;70,69,68,
3;71,68,69,
3;74,73,72,
3;75,72,73,
3;78,77,76,
3;79,76,77,
3;82,81,80,
3;83,80,81,
3;84,83,81,
3;87,86,85,
3;88,85,86,
3;91,90,89,
3;92,89,90,
3;95,94,93,
3;96,93,94,
3;99,98,97,
3;100,97,98,
3;103,102,101,
3;104,101,102,
3;107,106,105,
3;108,105,106,
3;111,110,109,
3;112,109,110,
3;115,114,113,
3;116,113,114,
3;117,113,116,
3;120,119,118,
3;121,118,119,
3;124,123,122,
3;125,122,123,
3;128,127,126,
3;129,126,127,
3;132,131,130,
3;133,130,131,
3;136,135,134,
3;137,134,135,
3;140,139,138,
3;141,138,139,
3;144,143,142,
3;145,142,143,
3;148,147,146,
3;149,146,147,
3;152,151,150,
3;153,150,151,
3;156,155,154,
3;157,154,155,
3;160,159,158,
3;163,162,161,
3;164,161,162,
3;165,161,164,
3;166,165,164,
3;167,165,166,
3;168,165,167,
3;169,161,165,
3;170,161,169,
3;171,161,170,
3;172,161,171,
3;173,161,172,
3;174,161,173,
3;175,161,174,
3;176,161,175,
3;177,161,176,
3;178,161,177,
3;179,161,178,
3;180,161,179,
3;183,182,181,
3;184,181,182,
3;185,181,184,
3;186,181,185,
3;187,186,185,
3;188,187,185,
3;189,188,185,
3;159,185,184,
3;158,159,184,
3;190,158,184,
3;191,190,184,
3;192,191,184,
3;193,192,184,
3;194,193,184,
3;195,194,184,
3;196,195,184,
3;197,196,184,
3;180,197,184,
3;161,180,184,
3;165,168,198,
3;201,200,199,
3;204,203,202,
3;205,202,203,
3;206,202,205,
3;207,206,205,
3;208,206,207,
3;209,206,208,
3;210,202,206,
3;211,202,210,
3;212,202,211,
3;213,202,212,
3;214,202,213,
3;215,202,214,
3;216,202,215,
3;217,202,216,
3;218,202,217,
3;219,202,218,
3;220,202,219,
3;221,202,220,
3;224,223,222,
3;225,222,223,
3;226,222,225,
3;227,222,226,
3;228,227,226,
3;229,228,226,
3;230,229,226,
3;200,226,225,
3;199,200,225,
3;231,199,225,
3;232,231,225,
3;233,232,225,
3;234,233,225,
3;235,234,225,
3;236,235,225,
3;237,236,225,
3;238,237,225,
3;221,238,225,
3;202,221,225,
3;206,209,239,
3;242,241,240,
3;243,240,241,
3;246,245,244,
3;247,244,245,
3;250,249,248,
3;251,248,249,
3;254,253,252,
3;255,252,253,
3;258,257,256,
3;259,256,257,
3;262,261,260,
3;263,260,261,
3;266,265,264,
3;267,264,265,
3;270,269,268,
3;271,268,269,
3;274,273,272,
3;275,272,273;;
MeshNormals {
276;
1.000000;0.000000;0.000000;
1.000000;0.000000;0.000000;
1.000000;0.000000;0.000000;
1.000000;0.000000;0.000000;
-1.000000;0.000000;0.000000;
-1.000000;0.000000;0.000000;
-1.000000;0.000000;0.000000;
-1.000000;0.000000;0.000000;
0.866025;-0.500000;-0.000000;
0.707107;-0.707107;0.000000;
0.866025;-0.500000;-0.000000;
0.707107;-0.707107;0.000000;
1.000000;0.000000;0.000000;
0.965926;-0.258819;-0.000000;
1.000000;0.000000;0.000000;
0.965926;-0.258819;-0.000000;
0.000000;1.000000;0.000000;
0.000000;1.000000;0.000000;
0.000000;1.000000;0.000000;
0.000000;1.000000;0.000000;
0.500000;-0.866025;0.000000;
0.707107;-0.707107;0.000000;
0.707107;-0.707107;0.000000;
0.500000;-0.866025;0.000000;
0.258819;-0.965926;0.000000;
0.500000;-0.866025;0.000000;
0.500000;-0.866025;0.000000;
0.258819;-0.965926;0.000000;
0.965926;-0.258819;-0.000000;
0.866025;-0.500000;-0.000000;
0.965926;-0.258819;-0.000000;
0.866025;-0.500000;-0.000000;
0.965926;0.258819;0.000000;
1.000000;0.000000;0.000000;
0.965926;0.258819;0.000000;
1.000000;0.000000;0.000000;
0.866025;0.500000;0.000000;
0.965926;0.258819;0.000000;
0.866025;0.500000;0.000000;
0.965926;0.258819;0.000000;
0.000000;-1.000000;0.000000;
0.258819;-0.965926;0.000000;
0.258819;-0.965926;0.000000;
0.000000;-1.000000;0.000000;
-0.965926;0.258819;0.000000;
-0.866025;0.500000;0.000000;
-0.965926;0.258819;0.000000;
-0.866025;0.500000;0.000000;
-0.965926;-0.258819;0.000000;
-1.000000;-0.000000;0.000000;
-0.965926;-0.258819;0.000000;
-1.000000;-0.000000;0.000000;
0.000000;1.000000;0.000000;
0.000000;1.000000;0.000000;
0.000000;1.000000;0.000000;
0.000000;1.000000;0.000000;
-0.258819;-0.965926;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
-0.258819;-0.965926;0.000000;
-1.000000;-0.000000;0.000000;
-0.965926;0.258819;0.000000;
-1.000000;-0.000000;0.000000;
-0.965926;0.258819;0.000000;
0.707107;0.707107;0.000000;
0.866025;0.500000;0.000000;
0.707107;0.707107;0.000000;
0.866025;0.500000;0.000000;
-0.707107;0.707107;0.000000;
-0.608761;0.793353;0.000000;
-0.608761;0.793353;0.000000;
-0.707107;0.707107;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
-0.500000;-0.866025;0.000000;
-0.258819;-0.965926;0.000000;
-0.258819;-0.965926;0.000000;
-0.500000;-0.866025;0.000000;
0.000000;0.000000;1.000000;
0.000000;0.000000;1.000000;
0.000000;0.000000;1.000000;
0.000000;0.000000;1.000000;
0.000000;0.000000;1.000000;
0.000000;1.000000;0.000000;
0.000000;1.000000;0.000000;
0.000000;1.000000;0.000000;
0.000000;1.000000;0.000000;
-0.707107;-0.707107;0.000000;
-0.500000;-0.866025;0.000000;
-0.500000;-0.866025;0.000000;
-0.707107;-0.707107;0.000000;
0.608761;0.793353;0.000000;
0.707107;0.707107;0.000000;
0.707107;0.707107;0.000000;
0.608761;0.793353;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
-0.866025;0.500000;0.000000;
-0.707107;0.707107;0.000000;
-0.866025;0.500000;0.000000;
-0.707107;0.707107;0.000000;
1.000000;0.000000;0.000000;
1.000000;0.000000;0.000000;
1.000000;0.000000;0.000000;
1.000000;0.000000;0.000000;
-0.707107;-0.707107;0.000000;
-0.866025;-0.500000;0.000000;
-0.707107;-0.707107;0.000000;
-0.866025;-0.500000;0.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
-1.000000;0.000000;0.000000;
-1.000000;0.000000;0.000000;
-1.000000;0.000000;0.000000;
-1.000000;0.000000;0.000000;
-1.000000;0.000000;0.000000;
-1.000000;0.000000;0.000000;
-1.000000;0.000000;0.000000;
-1.000000;0.000000;0.000000;
-0.866025;-0.500000;0.000000;
-0.965926;-0.258819;0.000000;
-0.866025;-0.500000;0.000000;
-0.965926;-0.258819;0.000000;
1.000000;0.000000;0.000000;
1.000000;0.000000;0.000000;
1.000000;0.000000;0.000000;
1.000000;0.000000;0.000000;
0.914760;-0.403998;-0.000000;
0.857631;-0.514265;-0.000000;
0.914760;-0.403998;-0.000000;
0.857631;-0.514265;-0.000000;
0.857631;-0.514265;-0.000000;
0.802981;-0.596005;0.000000;
0.857631;-0.514265;-0.000000;
0.802981;-0.596005;0.000000;
-0.537737;-0.843113;0.000000;
-0.614730;-0.788738;0.000000;
-0.537737;-0.843113;0.000000;
-0.614730;-0.788738;0.000000;
-0.000000;-1.000000;0.000000;
-0.000000;-1.000000;0.000000;
-0.000000;-1.000000;0.000000;
-0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;-1.000000;0.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
0.000000;0.000000;-1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
-0.000000;-0.000000;1.000000;
0.802981;-0.596005;0.000000;
0.707775;-0.706438;0.000000;
0.802981;-0.596005;0.000000;
0.707775;-0.706438;0.000000;
-0.999868;-0.016244;-0.000000;
-0.757072;-0.653332;0.000000;
-0.999868;-0.016244;-0.000000;
-0.757072;-0.653332;0.000000;
-0.602082;0.798434;-0.000000;
-0.999868;-0.016244;-0.000000;
-0.602082;0.798434;-0.000000;
-0.999868;-0.016244;-0.000000;
0.576555;-0.817058;0.000000;
0.707775;-0.706438;0.000000;
0.707775;-0.706438;0.000000;
0.576555;-0.817058;0.000000;
-0.602082;0.798434;-0.000000;
-0.390250;0.920709;-0.000000;
-0.602082;0.798434;-0.000000;
-0.390250;0.920709;-0.000000;
0.932076;0.362263;-0.000000;
0.932076;0.362263;-0.000000;
0.932076;0.362263;-0.000000;
0.932076;0.362263;-0.000000;
-0.962748;-0.270398;0.000000;
-0.996696;0.081228;0.000000;
-0.962748;-0.270398;0.000000;
-0.996696;0.081228;0.000000;
-0.614730;-0.788738;0.000000;
-0.962748;-0.270398;0.000000;
-0.614730;-0.788738;0.000000;
-0.962748;-0.270398;0.000000;
-0.537737;-0.843113;0.000000;
-0.684035;-0.729449;0.000000;
-0.684035;-0.729449;0.000000;
-0.537737;-0.843113;0.000000;;
176;
3;2,1,0;
3;3,0,1;
3;6,5,4;
3;7,4,5;
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
3;30,29,28;
3;31,28,29;
3;34,33,32;
3;35,32,33;
3;38,37,36;
3;39,36,37;
3;42,41,40;
3;43,40,41;
3;46,45,44;
3;47,44,45;
3;50,49,48;
3;51,48,49;
3;54,53,52;
3;55,52,53;
3;58,57,56;
3;59,56,57;
3;62,61,60;
3;63,60,61;
3;66,65,64;
3;67,64,65;
3;70,69,68;
3;71,68,69;
3;74,73,72;
3;75,72,73;
3;78,77,76;
3;79,76,77;
3;82,81,80;
3;83,80,81;
3;84,83,81;
3;87,86,85;
3;88,85,86;
3;91,90,89;
3;92,89,90;
3;95,94,93;
3;96,93,94;
3;99,98,97;
3;100,97,98;
3;103,102,101;
3;104,101,102;
3;107,106,105;
3;108,105,106;
3;111,110,109;
3;112,109,110;
3;115,114,113;
3;116,113,114;
3;117,113,116;
3;120,119,118;
3;121,118,119;
3;124,123,122;
3;125,122,123;
3;128,127,126;
3;129,126,127;
3;132,131,130;
3;133,130,131;
3;136,135,134;
3;137,134,135;
3;140,139,138;
3;141,138,139;
3;144,143,142;
3;145,142,143;
3;148,147,146;
3;149,146,147;
3;152,151,150;
3;153,150,151;
3;156,155,154;
3;157,154,155;
3;160,159,158;
3;163,162,161;
3;164,161,162;
3;165,161,164;
3;166,165,164;
3;167,165,166;
3;168,165,167;
3;169,161,165;
3;170,161,169;
3;171,161,170;
3;172,161,171;
3;173,161,172;
3;174,161,173;
3;175,161,174;
3;176,161,175;
3;177,161,176;
3;178,161,177;
3;179,161,178;
3;180,161,179;
3;183,182,181;
3;184,181,182;
3;185,181,184;
3;186,181,185;
3;187,186,185;
3;188,187,185;
3;189,188,185;
3;159,185,184;
3;158,159,184;
3;190,158,184;
3;191,190,184;
3;192,191,184;
3;193,192,184;
3;194,193,184;
3;195,194,184;
3;196,195,184;
3;197,196,184;
3;180,197,184;
3;161,180,184;
3;165,168,198;
3;201,200,199;
3;204,203,202;
3;205,202,203;
3;206,202,205;
3;207,206,205;
3;208,206,207;
3;209,206,208;
3;210,202,206;
3;211,202,210;
3;212,202,211;
3;213,202,212;
3;214,202,213;
3;215,202,214;
3;216,202,215;
3;217,202,216;
3;218,202,217;
3;219,202,218;
3;220,202,219;
3;221,202,220;
3;224,223,222;
3;225,222,223;
3;226,222,225;
3;227,222,226;
3;228,227,226;
3;229,228,226;
3;230,229,226;
3;200,226,225;
3;199,200,225;
3;231,199,225;
3;232,231,225;
3;233,232,225;
3;234,233,225;
3;235,234,225;
3;236,235,225;
3;237,236,225;
3;238,237,225;
3;221,238,225;
3;202,221,225;
3;206,209,239;
3;242,241,240;
3;243,240,241;
3;246,245,244;
3;247,244,245;
3;250,249,248;
3;251,248,249;
3;254,253,252;
3;255,252,253;
3;258,257,256;
3;259,256,257;
3;262,261,260;
3;263,260,261;
3;266,265,264;
3;267,264,265;
3;270,269,268;
3;271,268,269;
3;274,273,272;
3;275,272,273;;
}
MeshTextureCoords {
276;
302.181102,-351.706124;
1.000000,-565.879353;
1.000000,-351.706124;
302.181102,-565.879353;
-300.181102,-565.879353;
1.000000,-351.706124;
1.000000,-565.879353;
-300.181102,-351.706124;
302.181102,-988.252597;
1.000000,-1028.438220;
1.000000,-988.252597;
302.181102,-1028.438220;
302.181102,-914.342998;
1.000000,-954.528621;
1.000000,-914.342998;
302.181102,-954.528621;
424.949838,-0.000000;
93.949592,-301.181102;
93.949592,-0.000000;
424.949838,-301.181102;
302.181102,-962.408996;
1.000000,-922.223373;
302.181102,-922.223373;
1.000000,-962.408996;
302.181102,-832.162481;
1.000000,-791.976858;
302.181102,-791.976858;
1.000000,-832.162481;
302.181102,-985.564748;
1.000000,-1025.750370;
1.000000,-985.564748;
302.181102,-1025.750370;
302.181102,-779.440992;
1.000000,-819.626615;
1.000000,-779.440992;
302.181102,-819.626615;
302.181102,-590.052078;
1.000000,-630.237701;
1.000000,-590.052078;
302.181102,-630.237701;
302.181102,-646.574760;
1.000000,-606.389137;
302.181102,-606.389137;
1.000000,-646.574760;
-300.181102,-1025.750370;
1.000000,-985.564748;
1.000000,-1025.750370;
-300.181102,-985.564748;
-300.181102,-819.626615;
1.000000,-779.440992;
1.000000,-819.626615;
-300.181102,-779.440992;
1036.595864,-0.000000;
0.769092,-301.181102;
0.769092,-0.000000;
1036.595864,-301.181102;
-300.181102,418.293330;
1.000000,378.107707;
-300.181102,378.107707;
1.000000,418.293330;
-300.181102,-954.528621;
1.000000,-914.342998;
1.000000,-954.528621;
-300.181102,-914.342998;
302.181102,-359.082799;
1.000000,-399.268422;
1.000000,-359.082799;
302.181102,-399.268422;
1.000000,-962.408996;
-300.181102,-942.793013;
1.000000,-942.793013;
-300.181102,-962.408996;
-139.122294,-301.181102;
1.230908,-0.000000;
1.230908,-301.181102;
-139.122294,-0.000000;
-300.181102,162.875192;
1.000000,122.689569;
-300.181102,122.689569;
1.000000,162.875192;
-516.682478,0.961247;
-570.097962,-139.864638;
-624.831660,0.961247;
-463.215893,-60.995058;
-504.450161,-112.039253;
950.248804,-0.000000;
639.123066,-301.181102;
639.123066,-0.000000;
950.248804,-301.181102;
-300.181102,-102.273328;
1.000000,-142.458951;
-300.181102,-142.458951;
1.000000,-102.273328;
1.000000,-122.842969;
302.181102,-142.458951;
1.000000,-142.458951;
302.181102,-122.842969;
-948.248804,-301.181102;
-609.049703,-0.000000;
-609.049703,-301.181102;
-948.248804,-0.000000;
-300.181102,-1028.438220;
1.000000,-988.252597;
1.000000,-1028.438220;
-300.181102,-988.252597;
302.181102,-565.879353;
1.000000,-753.674628;
1.000000,-565.879353;
302.181102,-753.674628;
-300.181102,-399.268422;
1.000000,-359.082799;
1.000000,-399.268422;
-300.181102,-359.082799;
572.097962,-139.864638;
518.682478,0.961247;
626.831660,0.961247;
465.215893,-60.995058;
506.450161,-112.039253;
-422.474709,-301.181102;
-91.949592,-0.000000;
-91.949592,-301.181102;
-422.474709,-0.000000;
-300.181102,-753.674628;
1.000000,-565.879353;
1.000000,-753.674628;
-300.181102,-565.879353;
-300.181102,-1073.841903;
1.000000,0.961247;
1.000000,-1073.841903;
-300.181102,0.961247;
-300.181102,-630.237701;
1.000000,-590.052078;
1.000000,-630.237701;
-300.181102,-590.052078;
302.181102,0.961247;
1.000000,-1073.841903;
1.000000,0.961247;
302.181102,-1073.841903;
302.181102,-55.729851;
1.000000,-142.418657;
1.000000,-55.729851;
302.181102,-142.418657;
302.181102,-169.667955;
1.000000,-277.956986;
1.000000,-169.667955;
302.181102,-277.956986;
-300.181102,646.284355;
1.000000,742.398107;
1.000000,646.284355;
-300.181102,742.398107;
-624.831660,-301.181102;
-516.682478,0.000000;
-516.682478,-301.181102;
-624.831660,0.000000;
-1034.595864,-301.181102;
-919.648329,-0.000000;
-919.648329,-301.181102;
-1034.595864,-0.000000;
408.912304,-765.616082;
93.949592,-753.674628;
424.474709,-753.674628;
1036.595864,-1073.841903;
921.648329,0.961247;
1036.595864,0.961247;
930.034864,-101.944914;
950.248804,-565.879353;
873.408499,-179.784122;
784.230445,-215.632806;
639.123066,-351.706124;
950.248804,-753.674628;
611.049703,-753.674628;
626.612108,-765.616082;
651.075565,-797.497480;
666.453937,-834.624154;
671.699214,-874.465984;
666.453937,-914.307813;
651.075565,-951.434488;
626.612108,-983.315886;
594.730710,-1007.779343;
557.604035,-1023.157715;
517.762206,-1028.402992;
176.144420,-78.338184;
0.769092,0.961247;
141.122294,0.961247;
0.769092,-1073.841903;
93.949592,-565.879353;
242.915519,-163.591540;
326.104062,-281.955806;
424.949838,-351.706124;
424.949838,-565.879353;
384.448846,-797.497480;
369.070474,-834.624154;
363.825198,-874.465984;
369.070474,-914.307813;
384.448846,-951.434488;
408.912304,-983.315886;
440.793702,-1007.779343;
477.920376,-1023.157715;
639.123066,-565.879353;
-624.612108,-765.616082;
-948.248804,-753.674628;
-609.049703,-753.674628;
1.230908,-1073.841903;
-139.122294,0.961247;
1.230908,0.961247;
-174.144420,-78.338184;
-91.949592,-565.879353;
-240.915519,-163.591540;
-324.104062,-281.955806;
-422.949838,-351.706124;
-91.949592,-753.674628;
-422.474709,-753.674628;
-406.912304,-765.616082;
-382.448846,-797.497480;
-367.070474,-834.624154;
-361.825198,-874.465984;
-367.070474,-914.307813;
-382.448846,-951.434488;
-406.912304,-983.315886;
-438.793702,-1007.779343;
-475.920376,-1023.157715;
-515.762206,-1028.402992;
-928.034864,-101.944914;
-1034.595864,0.961247;
-919.648329,0.961247;
-1034.595864,-1073.841903;
-948.248804,-565.879353;
-871.408499,-179.784122;
-782.230445,-215.632806;
-637.123066,-351.706124;
-637.123066,-565.879353;
-649.075565,-797.497480;
-664.453937,-834.624154;
-669.699214,-874.465984;
-664.453937,-914.307813;
-649.075565,-951.434488;
-624.612108,-983.315886;
-592.730710,-1007.779343;
-555.604035,-1023.157715;
-422.949838,-565.879353;
302.181102,-272.945503;
1.000000,-417.619043;
1.000000,-272.945503;
302.181102,-417.619043;
-300.181102,257.109441;
1.000000,338.946226;
1.000000,257.109441;
-300.181102,338.946226;
-300.181102,-404.777011;
1.000000,-339.158609;
1.000000,-404.777011;
-300.181102,-339.158609;
302.181102,-549.169694;
1.000000,-428.192034;
302.181102,-428.192034;
1.000000,-549.169694;
1.000000,-509.095761;
-300.181102,-580.397135;
-300.181102,-509.095761;
1.000000,-580.397135;
302.181102,227.611433;
1.000000,76.523023;
1.000000,227.611433;
302.181102,76.523023;
-300.181102,-177.071337;
1.000000,-73.824003;
1.000000,-177.071337;
-300.181102,-73.824003;
-300.181102,367.837358;
1.000000,464.094761;
1.000000,367.837358;
-300.181102,464.094761;
-300.181102,423.826267;
1.000000,224.898895;
-300.181102,224.898895;
1.000000,423.826267;;
}
MeshMaterialList {
1;
176;
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
