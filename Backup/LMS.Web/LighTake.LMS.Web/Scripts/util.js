function isNull(obj) {
    return typeof (obj) == "undefined" || obj == null;
};

$.format = function (source, params) {
    if (isNull(source)) {
        source = "";
    }
    source = source + "";
    if (arguments.length == 1)
        return function () {
            var args = $.makeArray(arguments);
            args.unshift(source);
            return $.validator.format.apply(this, args);
        };
    if (arguments.length > 2 && params.constructor != Array) {
        params = $.makeArray(arguments).slice(1);
    }
    if (params.constructor != Array) {
        params = [params];
    }
    $.each(params, function (i, n) {
        source = source.replace(new RegExp("\\{" + i + "\\}", "g"), isNull(n) ? '' : n);
    });
    return source;
};


function setSelectedValue(id, value) {
    if (!value) {
        return;
    }
    value = value.toString();
    var count = $("#"+id)[0].length;
    var s = $("#"+id).get(0);
    s.selectedIndex = -1;

    for (var i = 0; i < count; i++) {
        if (s.options[i].value == value) {
            //s.options[i].selected = true;
            s.selectedIndex = i;
            break;
        }
    }
}


var UTF8Encoder =
{
    getBytes: function (s)
    {
        /// <summary>
        /// Get array of bytes.
        /// </summary>
        var bytes = new Array();
        var c;
        for (var i = 0; i < s.length; i++)
        {
            c = s.charCodeAt(i);
            // Convert char code to bytes.
            if (c < 0x80)
            {
                bytes.push(c);
            } else if (c < 0x800)
            {
                bytes.push(0xC0 | c >> 6);
                bytes.push(0x80 | c & 0x3F);
            } else if (c < 0x10000)
            {
                bytes.push(0xE0 | c >> 12);
                bytes.push(0x80 | c >> 6 & 0x3F);
                bytes.push(0x80 | c & 0x3F);
            } else if (c < 0x200000)
            {
                bytes.push(0xF0 | c >> 18);
                bytes.push(0x80 | c >> 12 & 0x3F);
                bytes.push(0x80 | c >> 6 & 0x3F);
                bytes.push(0x80 | c & 0x3F);
            } else
            {
                // If char is unknown then push "?".
                bytes.push(0x3F);
            }
        }
        return bytes;
    },
    //---------------------------------------------------------
    getString: function (bytes)
    {
        /// <summary>
        /// Get string from array of bytes.
        /// </summary>
        var s = new String;
        var b;
        var b1;
        var b2;
        var b3;
        var b4;
        var bE;
        var ln = bytes.length;
        for (var i = 0; i < ln; i++)
        {
            b = bytes[i];
            if (!b)
            {
                continue;
            }
            if (b < 0x80)
            {
                // Char represended by 1 byte.
                s += (b > 0) ? String.fromCharCode(b) : "";
            } else if (b < 0xC0)
            {
                // Byte 2,3,4 of unicode char.
            } else if (b < 0xE0)
            {
                // Char represended by 2 bytes.
                if (ln > i + 1)
                {
                    b1 = (b & 0x1F); i++;
                    b2 = (bytes[i] & 0x3F);
                    bE = (b1 << 6) | b2;
                    s += String.fromCharCode(bE);
                }
            } else if (b < 0xF0)
            {
                // Char represended by 3 bytes.
                if (ln > i + 2)
                {
                    b1 = (b & 0xF); i++;
                    b2 = (bytes[i] & 0x3F); i++;
                    b3 = (bytes[i] & 0x3F);
                    bE = (b1 << 12) | (b2 << 6) | b3;
                    s += String.fromCharCode(bE);
                }
            } else if (b < 0xF8)
            {
                // Char represended by 4 bytes.
                if (ln > i + 3)
                {
                    b1 = (b & 0x7); i++;
                    b2 = (bytes[i] & 0x3F); i++;
                    b3 = (bytes[i] & 0x3F); i++;
                    b4 = (bytes[i] & 0x3F);
                    bE = (b1 << 18) | (b2 << 12)(b3 << 6) | b4;
                    s += String.fromCharCode(bE);
                }
            } else
            {
                s += "?";
            }
        }
        return s;
    }
};

var UrlDecoderHelper = function ()
{
    // Fields
    var _byteBuffer;
    var _charBuffer = [];
    var _numBytes = 0;
    var str;

    this.addByte = function (b)
    {
        if (!_byteBuffer)
        {
            _byteBuffer = [];
        }
        _byteBuffer[_numBytes++] = b;
    };

    this.addChar = function (ch)
    {
        if (_numBytes > 0)
        {
            this.flushBytes();
        }
        _charBuffer.push(ch);
    };

    this.flushBytes = function ()
    {
        if (_numBytes > 0)
        {
            str = UTF8Encoder.getString(_byteBuffer);
            _numBytes = 0;
            _byteBuffer = [];
            for (var i = 0; i < str.length; i++)
            {
                _charBuffer.push(str.charAt(i));
            }
        }
    };

    this.getString = function ()
    {
        if (_numBytes > 0)
        {
            this.flushBytes();
        }
        return _charBuffer.join('');
    };

};

var HttpUtility =
{

    hexToInt: function (h)
    {
        if (!h)
        {
            return -1;
        }
        h = h.toString().charCodeAt(0);

        if ((h >= '0'.charCodeAt(0)) && (h <= '9'.charCodeAt(0)))
        {
            return parseInt(h - '0'.charCodeAt(0));
        }
        if ((h >= 'a'.charCodeAt(0)) && (h <= 'f'.charCodeAt(0)))
        {
            return parseInt((h - 'a'.charCodeAt(0)) + 10);
        }
        if ((h >= 'A'.charCodeAt(0)) && (h <= 'F'.charCodeAt(0)))
        {
            return parseInt((h - 'A'.charCodeAt(0)) + 10);
        }
        return -1;
    },

    urlEncode: function (string)
    {
        if (typeof (string) == 'undefined' || string == null)
        {
            return '';
        }
        string = string.toString();
        if (string.length == 0)
        {
            return '';
        }
        string = string.replace(/\r\n/g, "\n");
        var utftext = [];

        for (var n = 0; n < string.length; n++)
        {

            var c = string.charCodeAt(n);

            if (c < 128)
            {
                utftext.push(String.fromCharCode(c));
            }
            else if ((c > 127) && (c < 2048))
            {
                utftext.push(String.fromCharCode((c >> 6) | 192));
                utftext.push(String.fromCharCode((c & 63) | 128));
            }
            else
            {
                utftext.push(String.fromCharCode((c >> 12) | 224));
                utftext.push(String.fromCharCode(((c >> 6) & 63) | 128));
                utftext.push(String.fromCharCode((c & 63) | 128));
            }

        }

        return utftext.length > 0 ? escape(utftext.join('')) : '';
    },

    urlDecode: function (value)
    {
        if (!value)
        {
            return '';
        }
        value = value.toString();
        if (value.length == 0)
        {
            return '';
        }
        var length = value.length;
        var decoder = new UrlDecoderHelper();
        var ch;
        var num3;
        var num4;
        var num5;
        var num6;
        var num7;
        var num8;
        var b;

        for (var i = 0; i < length; i++)
        {
            ch = value.charAt(i);
            if (ch == '+')
            {
                ch = ' ';
            }
            else if ((ch == '%') && (i < (length - 2)))
            {
                if ((value.charAt(i + 1) == 'u') && (i < (length - 5)))
                {
                    num3 = this.hexToInt(value.charAt(i + 2));
                    num4 = this.hexToInt(value.charAt(i + 3));
                    num5 = this.hexToInt(value.charAt(i + 4));
                    num6 = this.hexToInt(value.charAt(i + 5));
                    if (((num3 < 0) || (num4 < 0)) || ((num5 < 0) || (num6 < 0)))
                    {
                        if ((ch.charCodeAt(0) & 0xff80) == 0)
                        {
                            decoder.addByte(ch.charCodeAt(0) & 255);
                        }
                        else
                        {
                            decoder.addChar(ch);
                        }
                    }
                    ch = ((((num3 << 12) | (num4 << 8)) | (num5 << 4)) | num6) + '';
                    i += 5;
                    decoder.addChar(ch);
                    continue;
                }
                num7 = this.hexToInt(value.charAt(i + 1));
                num8 = this.hexToInt(value.charAt(i + 2));
                if ((num7 >= 0) && (num8 >= 0))
                {
                    b = ((num7 << 4) | num8) & 255;
                    i += 2;
                    decoder.addByte(b);
                    continue;
                }

            } // else if ((ch == '%') && (i < (length - 2)))

            if ((ch.charCodeAt(0) & 0xff80) == 0)
            {
                decoder.addByte(ch.charCodeAt(0) & 255);
            }
            else
            {
                decoder.addChar(ch);
            }
        }
        return decoder.getString();

    }  // urlDecode

};


