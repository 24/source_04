using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace pb.Data.Mongo.Serializers
{
    // class created from MongoDB.Bson.Serialization.Serializers.DateTimeSerializer
    public class DateSerializer : BsonBaseSerializer
    {
        private static bool _trace = false;

        //        public DateSerializer()
        //#pragma warning disable 618
        //            : this(DateTimeSerializationOptions.Defaults)
        //#pragma warning restore
        //        {
        //        }

        //        public DateSerializer(DateTimeSerializationOptions defaultSerializationOptions)
        //            : base(defaultSerializationOptions)
        //        {
        //        }

        //public DateSerializer()
        //{
        //}

        public static bool Trace { get { return _trace; } set { _trace = value; } }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            if (_trace)
                pb.Trace.WriteLine("DateSerializer.Deserialize()");

            // DateTime
            VerifyTypes(nominalType, actualType, typeof(Date));
            //var dateTimeSerializationOptions = EnsureSerializationOptions<DateTimeSerializationOptions>(options);

            var bsonType = bsonReader.GetCurrentBsonType();
            //DateTime value;
            Date value;
            switch (bsonType)
            {
                //case BsonType.DateTime:
                //    // use an intermediate BsonDateTime so MinValue and MaxValue are handled correctly
                //    value = new BsonDateTime(bsonReader.ReadDateTime()).ToUniversalTime();
                //    break;
                //case BsonType.Document:
                //    bsonReader.ReadStartDocument();
                //    bsonReader.ReadDateTime("DateTime"); // ignore value (use Ticks instead)
                //    value = new DateTime(bsonReader.ReadInt64("Ticks"), DateTimeKind.Utc);
                //    bsonReader.ReadEndDocument();
                //    break;
                //case BsonType.Int64:
                //    value = DateTime.SpecifyKind(new DateTime(bsonReader.ReadInt64()), DateTimeKind.Utc);
                //    break;
                case BsonType.String:
                    //value = DateTime.SpecifyKind(DateTime.ParseExact(bsonReader.ReadString(), "yyyy-MM-dd", null), DateTimeKind.Utc);
                    value = Date.ParseExact(bsonReader.ReadString(), "yyyy-MM-dd", null);
                    // note: we're not using XmlConvert because of bugs in Mono
                    //if (dateTimeSerializationOptions.DateOnly)
                    //{
                    //    value = DateTime.SpecifyKind(DateTime.ParseExact(bsonReader.ReadString(), "yyyy-MM-dd", null), DateTimeKind.Utc);
                    //}
                    //else
                    //{
                    //    var formats = new string[] { "yyyy-MM-ddK", "yyyy-MM-ddTHH:mm:ssK", "yyyy-MM-ddTHH:mm:ss.FFFFFFFK" };
                    //    value = DateTime.ParseExact(bsonReader.ReadString(), formats, null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
                    //}
                    break;
                default:
                    //var message = string.Format("Cannot deserialize DateTime from BsonType {0}.", bsonType);
                    throw new PBException("Cannot deserialize DateTime from BsonType {0}.", bsonType);
            }

            //if (dateTimeSerializationOptions.DateOnly)
            //{
            //    if (value.TimeOfDay != TimeSpan.Zero)
            //    {
            //        throw new FileFormatException("TimeOfDay component for DateOnly DateTime value is not zero.");
            //    }
            //    value = DateTime.SpecifyKind(value, dateTimeSerializationOptions.Kind); // not ToLocalTime or ToUniversalTime!
            //}
            //else
            //{
            //    switch (dateTimeSerializationOptions.Kind)
            //    {
            //        case DateTimeKind.Local:
            //        case DateTimeKind.Unspecified:
            //            value = DateTime.SpecifyKind(BsonUtils.ToLocalTime(value), dateTimeSerializationOptions.Kind);
            //            break;
            //        case DateTimeKind.Utc:
            //            value = BsonUtils.ToUniversalTime(value);
            //            break;
            //    }
            //}

            return value;
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            //var dateTime = (DateTime)value;
            //var dateTimeSerializationOptions = EnsureSerializationOptions<DateTimeSerializationOptions>(options);

            //DateTime utcDateTime;
            //if (dateTimeSerializationOptions.DateOnly)
            //{
            //    if (dateTime.TimeOfDay != TimeSpan.Zero)
            //    {
            //        throw new BsonSerializationException("TimeOfDay component is not zero.");
            //    }
            //    utcDateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc); // not ToLocalTime
            //}
            //else
            //{
            //    utcDateTime = BsonUtils.ToUniversalTime(dateTime);
            //}
            //var millisecondsSinceEpoch = BsonUtils.ToMillisecondsSinceEpoch(utcDateTime);

            //switch (dateTimeSerializationOptions.Representation)
            //{
            //    case BsonType.DateTime:
            //        bsonWriter.WriteDateTime(millisecondsSinceEpoch);
            //        break;
            //    case BsonType.Document:
            //        bsonWriter.WriteStartDocument();
            //        bsonWriter.WriteDateTime("DateTime", millisecondsSinceEpoch);
            //        bsonWriter.WriteInt64("Ticks", utcDateTime.Ticks);
            //        bsonWriter.WriteEndDocument();
            //        break;
            //    case BsonType.Int64:
            //        bsonWriter.WriteInt64(utcDateTime.Ticks);
            //        break;
            //    case BsonType.String:
            //        if (dateTimeSerializationOptions.DateOnly)
            //        {
            //            bsonWriter.WriteString(dateTime.ToString("yyyy-MM-dd"));
            //        }
            //        else
            //        {
            //            // we're not using XmlConvert.ToString because of bugs in Mono
            //            if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue)
            //            {
            //                // serialize MinValue and MaxValue as Unspecified so we do NOT get the time zone offset
            //                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            //            }
            //            else if (dateTime.Kind == DateTimeKind.Unspecified)
            //            {
            //                // serialize Unspecified as Local se we get the time zone offset
            //                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            //            }
            //            bsonWriter.WriteString(dateTime.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFK"));
            //        }
            //        break;
            //    default:
            //        var message = string.Format("'{0}' is not a valid DateTime representation.", dateTimeSerializationOptions.Representation);
            //        throw new BsonSerializationException(message);
            //}

            if (_trace)
                pb.Trace.WriteLine("DateSerializer.Serialize()");

            bsonWriter.WriteString(((Date)value).ToString("yyyy-MM-dd"));
        }
    }
}
