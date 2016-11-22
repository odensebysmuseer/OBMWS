﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq
{
  /// <summary>
  /// Represents a writer that provides a fast, non-cached, forward-only way of generating Json data.
  /// </summary>
  public class JTokenWriter : JsonWriter
  {
    private JContainer _token;
    private JContainer _parent;
    // used when writer is writing single value and the value has no containing parent
    private JValue _value;

    /// <summary>
    /// Gets the token being writen.
    /// </summary>
    /// <value>The token being writen.</value>
    public JToken Token
    {
      get
      {
        if (_token != null)
          return _token;

        return _value;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JTokenWriter"/> class writing to the given <see cref="JContainer"/>.
    /// </summary>
    /// <param name="container">The container being written to.</param>
    public JTokenWriter(JContainer container)
    {
      ValidationUtils.ArgumentNotNull(container, "container");

      _token = container;
      _parent = container;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JTokenWriter"/> class.
    /// </summary>
    public JTokenWriter()
    {
    }

    /// <summary>
    /// Flushes whatever is in the buffer to the underlying streams and also flushes the underlying stream.
    /// </summary>
    public override void Flush()
    {
    }

    /// <summary>
    /// Closes this stream and the underlying stream.
    /// </summary>
    public override void Close()
    {
      base.Close();
    }

    /// <summary>
    /// Writes the beginning of a Json object.
    /// </summary>
    public override void WriteStartObject(OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteStartObject(mode);

      AddParent(new JObject());
    }

    private void AddParent(JContainer container)
    {
      if (_parent == null)
        _token = container;
      else
        _parent.Add(container);

      _parent = container;
    }

    private void RemoveParent()
    {
      _parent = _parent.Parent;

      if (_parent != null && _parent.Type == JTokenType.Property)
        _parent = _parent.Parent;
    }

    /// <summary>
    /// Writes the beginning of a Json array.
    /// </summary>
    public override void WriteStartArray(OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteStartArray(mode);

      AddParent(new JArray());
    }

    /// <summary>
    /// Writes the start of a constructor with the given name.
    /// </summary>
    /// <param name="name">The name of the constructor.</param>
    public override void WriteStartConstructor(string name, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteStartConstructor(name,mode);

      AddParent(new JConstructor(name));
    }

    /// <summary>
    /// Writes the end.
    /// </summary>
    /// <param name="token">The token.</param>
    protected override void WriteEnd(JsonToken token)
    {
      RemoveParent();
    }

    /// <summary>
    /// Writes the property name of a name/value pair on a Json object.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    public override void WritePropertyName(string name, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WritePropertyName(name,mode);

      AddParent(new JProperty(name));
    }

    private void AddValue(object value, JsonToken token)
    {
      AddValue(new JValue(value), token);
    }

    internal void AddValue(JValue value, JsonToken token)
    {
      if (_parent != null)
      {
        _parent.Add(value);

        if (_parent.Type == JTokenType.Property)
          _parent = _parent.Parent;
      }
      else
      {
        _value = value;
      }
    }

    #region WriteValue methods
    /// <summary>
    /// Writes a null value.
    /// </summary>
    public override void WriteNull(OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteNull(mode);
      AddValue(null, JsonToken.Null);
    }

    /// <summary>
    /// Writes an undefined value.
    /// </summary>
    public override void WriteUndefined(OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteUndefined(mode);
      AddValue(null, JsonToken.Undefined);
    }

    /// <summary>
    /// Writes raw JSON.
    /// </summary>
    /// <param name="json">The raw JSON to write.</param>
    public override void WriteRaw(string json)
    {
      base.WriteRaw(json);
      AddValue(new JRaw(json), JsonToken.Raw);
    }

    /// <summary>
    /// Writes out a comment <code>/*...*/</code> containing the specified text.
    /// </summary>
    /// <param name="text">Text to place inside the comment.</param>
    public override void WriteComment(string text, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteComment(text,mode);
      AddValue(JValue.CreateComment(text), JsonToken.Comment);
    }

    /// <summary>
    /// Writes a <see cref="String"/> value.
    /// </summary>
    /// <param name="value">The <see cref="String"/> value to write.</param>
    public override void WriteValue(string value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteValue(value,mode);
      AddValue(value ?? string.Empty, JsonToken.String);
    }

    /// <summary>
    /// Writes a <see cref="Int32"/> value.
    /// </summary>
    /// <param name="value">The <see cref="Int32"/> value to write.</param>
    public override void WriteValue(int value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteValue(value,mode);
      AddValue(value, JsonToken.Integer);
    }

    /// <summary>
    /// Writes a <see cref="UInt32"/> value.
    /// </summary>
    /// <param name="value">The <see cref="UInt32"/> value to write.</param>
    [CLSCompliant(false)]
#pragma warning disable CS3021 // 'JTokenWriter.WriteValue(uint)' does not need a CLSCompliant attribute because the assembly does not have a CLSCompliant attribute
    public override void WriteValue(uint value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
#pragma warning restore CS3021 // 'JTokenWriter.WriteValue(uint)' does not need a CLSCompliant attribute because the assembly does not have a CLSCompliant attribute
    {
      base.WriteValue(value,mode);
      AddValue(value, JsonToken.Integer);
    }

    /// <summary>
    /// Writes a <see cref="Int64"/> value.
    /// </summary>
    /// <param name="value">The <see cref="Int64"/> value to write.</param>
    public override void WriteValue(long value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteValue(value,mode);
      AddValue(value, JsonToken.Integer);
    }

    /// <summary>
    /// Writes a <see cref="UInt64"/> value.
    /// </summary>
    /// <param name="value">The <see cref="UInt64"/> value to write.</param>
    [CLSCompliant(false)]
#pragma warning disable CS3021 // 'JTokenWriter.WriteValue(ulong)' does not need a CLSCompliant attribute because the assembly does not have a CLSCompliant attribute
    public override void WriteValue(ulong value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
#pragma warning restore CS3021 // 'JTokenWriter.WriteValue(ulong)' does not need a CLSCompliant attribute because the assembly does not have a CLSCompliant attribute
    {
      base.WriteValue(value,mode);
      AddValue(value, JsonToken.Integer);
    }

    /// <summary>
    /// Writes a <see cref="Single"/> value.
    /// </summary>
    /// <param name="value">The <see cref="Single"/> value to write.</param>
    public override void WriteValue(float value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteValue(value,mode);
      AddValue(value, JsonToken.Float);
    }

    /// <summary>
    /// Writes a <see cref="Double"/> value.
    /// </summary>
    /// <param name="value">The <see cref="Double"/> value to write.</param>
    public override void WriteValue(double value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteValue(value,mode);
      AddValue(value, JsonToken.Float);
    }

    /// <summary>
    /// Writes a <see cref="Boolean"/> value.
    /// </summary>
    /// <param name="value">The <see cref="Boolean"/> value to write.</param>
    public override void WriteValue(bool value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteValue(value,mode);
      AddValue(value, JsonToken.Boolean);
    }

    /// <summary>
    /// Writes a <see cref="Int16"/> value.
    /// </summary>
    /// <param name="value">The <see cref="Int16"/> value to write.</param>
    public override void WriteValue(short value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteValue(value,mode);
      AddValue(value, JsonToken.Integer);
    }

    /// <summary>
    /// Writes a <see cref="UInt16"/> value.
    /// </summary>
    /// <param name="value">The <see cref="UInt16"/> value to write.</param>
    [CLSCompliant(false)]
#pragma warning disable CS3021 // 'JTokenWriter.WriteValue(ushort)' does not need a CLSCompliant attribute because the assembly does not have a CLSCompliant attribute
    public override void WriteValue(ushort value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
#pragma warning restore CS3021 // 'JTokenWriter.WriteValue(ushort)' does not need a CLSCompliant attribute because the assembly does not have a CLSCompliant attribute
    {
      base.WriteValue(value,mode);
      AddValue(value, JsonToken.Integer);
    }

    /// <summary>
    /// Writes a <see cref="Char"/> value.
    /// </summary>
    /// <param name="value">The <see cref="Char"/> value to write.</param>
    public override void WriteValue(char value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteValue(value,mode);
      AddValue(value.ToString(), JsonToken.String);
    }

    /// <summary>
    /// Writes a <see cref="Byte"/> value.
    /// </summary>
    /// <param name="value">The <see cref="Byte"/> value to write.</param>
    public override void WriteValue(byte value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteValue(value,mode);
      AddValue(value, JsonToken.Integer);
    }

    /// <summary>
    /// Writes a <see cref="SByte"/> value.
    /// </summary>
    /// <param name="value">The <see cref="SByte"/> value to write.</param>
    [CLSCompliant(false)]
#pragma warning disable CS3021 // 'JTokenWriter.WriteValue(sbyte)' does not need a CLSCompliant attribute because the assembly does not have a CLSCompliant attribute
    public override void WriteValue(sbyte value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
#pragma warning restore CS3021 // 'JTokenWriter.WriteValue(sbyte)' does not need a CLSCompliant attribute because the assembly does not have a CLSCompliant attribute
    {
      base.WriteValue(value,mode);
      AddValue(value, JsonToken.Integer);
    }

    /// <summary>
    /// Writes a <see cref="Decimal"/> value.
    /// </summary>
    /// <param name="value">The <see cref="Decimal"/> value to write.</param>
    public override void WriteValue(decimal value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteValue(value,mode);
      AddValue(value, JsonToken.Float);
    }

    /// <summary>
    /// Writes a <see cref="DateTime"/> value.
    /// </summary>
    /// <param name="value">The <see cref="DateTime"/> value to write.</param>
    public override void WriteValue(DateTime value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteValue(value,mode);
      AddValue(value, JsonToken.Date);
    }

#if !PocketPC && !NET20
    /// <summary>
    /// Writes a <see cref="DateTimeOffset"/> value.
    /// </summary>
    /// <param name="value">The <see cref="DateTimeOffset"/> value to write.</param>
    public override void WriteValue(DateTimeOffset value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteValue(value,mode);
      AddValue(value, JsonToken.Date);
    }
#endif

    /// <summary>
    /// Writes a <see cref="T:Byte[]"/> value.
    /// </summary>
    /// <param name="value">The <see cref="T:Byte[]"/> value to write.</param>
    public override void WriteValue(byte[] value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteValue(value,mode);
      AddValue(value, JsonToken.Bytes);
    }

    /// <summary>
    /// Writes a <see cref="TimeSpan"/> value.
    /// </summary>
    /// <param name="value">The <see cref="TimeSpan"/> value to write.</param>
    public override void WriteValue(TimeSpan value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteValue(value,mode);
      AddValue(value, JsonToken.String);
    }

    /// <summary>
    /// Writes a <see cref="Guid"/> value.
    /// </summary>
    /// <param name="value">The <see cref="Guid"/> value to write.</param>
    public override void WriteValue(Guid value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteValue(value,mode);
      AddValue(value, JsonToken.String);
    }

    /// <summary>
    /// Writes a <see cref="Uri"/> value.
    /// </summary>
    /// <param name="value">The <see cref="Uri"/> value to write.</param>
    public override void WriteValue(Uri value, OBMWS.PrintMode mode = OBMWS.PrintMode.ValueCell)
    {
      base.WriteValue(value,mode);
      AddValue(value, JsonToken.String);
    }

        internal override void postFormating()
        {

        }
        #endregion
    }
}