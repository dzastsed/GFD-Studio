﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AtlusGfdLib
{
    public sealed class TextureDictionary : Resource, IDictionary<string, Texture>
    {
        private readonly Dictionary<string, Texture> mDictionary;

        public TextureDictionary( uint version )
            : base( ResourceType.TextureDictionary, version )
        {
            mDictionary = new Dictionary<string, Texture>();
        }

        /// <summary>
        /// Converts the given texture dictionary to a field texture archive, and returns a new texture dictionary filled with dummy textures for each texture in the given texture dictionary.
        /// </summary>
        /// <param name="textureDictionary"></param>
        /// <param name="archiveFilePath"></param>
        /// <returns></returns>
        public static TextureDictionary ConvertToFieldTextureArchive( TextureDictionary textureDictionary, string archiveFilePath )
        {
            var archiveBuilder = new ArchiveBuilder();

            // Create bgTexArcData00.txt
            var fieldTextureArchiveDataInfoStream = new MemoryStream();
            using ( var streamWriter = new StreamWriter( fieldTextureArchiveDataInfoStream, Encoding.Default, 4096, true ) )
            {
                streamWriter.WriteLine( "1," );
                streamWriter.WriteLine( $"{textureDictionary.Count}," );
            }

            archiveBuilder.AddFile( "bgTexArcData00.txt", fieldTextureArchiveDataInfoStream );

            // Convert textures
            foreach ( var texture in textureDictionary.Textures )
            {
                var textureInfo = TextureUtillities.GetTextureInfo( texture );
                var texturePixelData = TextureUtillities.GetRawPixelData( texture );

                // Create field texture & save it
                var fieldTexture = new FieldTexture( textureInfo.PixelFormat, (byte)textureInfo.MipMapCount, ( short )textureInfo.Width, ( short )textureInfo.Height, texturePixelData );
                var textureStream = new MemoryStream();
                fieldTexture.Save( textureStream );
                archiveBuilder.AddFile( texture.Name, textureStream );
            }

            // Finally build archive file
            archiveBuilder.BuildFile( archiveFilePath );

            // Dummy out textures in texture dictionary
            var newTextureDictionary = new TextureDictionary( textureDictionary.Version );
            foreach ( var texture in textureDictionary.Textures )
                newTextureDictionary.Add( new Texture( texture.Name, TextureFormat.DDS, Texture.DummyTextureData ) );

            return newTextureDictionary;
        }

        public Texture this[string name]
        {
            get => mDictionary[name];
            set => mDictionary[name] = value;
        }

        public ICollection<Texture> Textures => mDictionary.Values;

        public void Add( Texture texture ) => mDictionary[texture.Name] = texture;

        public bool ContainsTexture( string name ) => mDictionary.ContainsKey( name );

        public bool ContainsTexture( Texture material ) => mDictionary.ContainsValue( material );

        public bool Remove( Texture texture ) => mDictionary.Remove( texture.Name );

        public bool TryGetTexture( string name, out Texture texture )
        {
            return mDictionary.TryGetValue( name, out texture );
        }

        #region IDictionary implementation 

        public int Count => mDictionary.Count;

        public ICollection<string> Keys => ( ( IDictionary<string, Texture> )mDictionary ).Keys;

        public ICollection<Texture> Values => ( ( IDictionary<string, Texture> )mDictionary ).Values;

        public bool IsReadOnly => ( ( IDictionary<string, Texture> )mDictionary ).IsReadOnly;

        public void Clear() => mDictionary.Clear();

        public bool Remove( string name ) => mDictionary.Remove( name );

        public bool ContainsKey( string key )
        {
            return mDictionary.ContainsKey( key );
        }

        public void Add( string key, Texture value )
        {
            mDictionary.Add( key, value );
        }

        public bool TryGetValue( string key, out Texture value )
        {
            return mDictionary.TryGetValue( key, out value );
        }

        public void Add( KeyValuePair<string, Texture> item )
        {
            ( ( IDictionary<string, Texture> )mDictionary ).Add( item );
        }

        public bool Contains( KeyValuePair<string, Texture> item )
        {
            return ( ( IDictionary<string, Texture> )mDictionary ).Contains( item );
        }

        public void CopyTo( KeyValuePair<string, Texture>[] array, int arrayIndex )
        {
            ( ( IDictionary<string, Texture> )mDictionary ).CopyTo( array, arrayIndex );
        }

        public bool Remove( KeyValuePair<string, Texture> item )
        {
            return ( ( IDictionary<string, Texture> )mDictionary ).Remove( item );
        }

        public IEnumerator<KeyValuePair<string, Texture>> GetEnumerator()
        {
            return ( ( IDictionary<string, Texture> )mDictionary ).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IDictionary<string, Texture> )mDictionary ).GetEnumerator();
        }

        #endregion
    }
}