/**
* jQuery ligerUI 1.2.0
* 
* http://ligerui.com
*  
* Author daomi 2013 [ gd_star@163.com ] 
* 
*/
(function ($)
{

    $.fn.ligerForm = function ()
    {
        return $.ligerui.run.call(this, "ligerForm", arguments);
    };

    $.ligerui.getConditions = function (form)
    {
        if (!form) return null;
        var conditions = [];
        $(":input", form).filter(".condition,.field").each(function ()
        {
            var value = $(this).val() || $(this).attr("value");
            if (!this.name || !value) return;
            conditions.push({
                op: $(this).attr("op") || "like",
                field: this.name,
                value: value,
                type: $(this).attr("vt") || "string"
            });
        });
        return conditions;
    };

    $.ligerDefaults = $.ligerDefaults || {};
    $.ligerDefaults.Form = {
        //控件宽度
        inputWidth: 180,
        //标签宽度
        labelWidth: 90,
        //间隔宽度
        space: 40,
        rightToken: '：',
        //标签对齐方式
        labelAlign: 'left',
        //控件对齐方式
        align: 'left',
        //字段
        fields: [],
        //创建的表单元素是否附加ID
        appendID: true,
        //生成表单元素ID的前缀
        prefixID: "",
        //json解析函数
        toJSON: $.ligerui.toJSON,
        labelCss: null,
        fieldCss: null,
        spaceCss: null,
        onAfterSetFields: null,
        buttons: null,              //按钮组
        readonly:false              //是否只读
    };

    $.ligerDefaults.Form_fields = {
        name: null,             //字段name
        type: null,             //表单类型
        editor: null,           //编辑器扩展类型
        label: null,            //Label
        newline: true,          //换行显示
        op: null,               //操作符 附加到input
        vt: null,               //值类型 附加到input
        attr: null             //属性列表 附加到input
    };

    $.ligerDefaults.Form_editor = {
        textFieldName: null    //文本框name 
    };

    //@description 默认表单编辑器构造器扩展(如果创建的表单效果不满意 建议重载)
    //@param {jinput} 表单元素jQuery对象 比如input、select、textarea 
    $.ligerDefaults.Form.editorBulider = function (jinput)
    {
        //这里this就是form的ligerui对象
        var g = this, p = this.options;
        var options = {}, field = null;
        var fieldIndex = jinput.attr("fieldindex"), ltype = jinput.attr("ltype");
        if (fieldIndex != null)
        {
            field = g.getField(fieldIndex); 
            if (field && g.editors && g.editors[field.type])
            { 
                g.editors[field.type].call(g, jinput, field);
                return;
            }
        }
        field = field || {};
        if (p.readonly) options.readonly = true;
        options = $.extend({
            width: (field.width || p.inputWidth) - 2
        }, field.options, field.editor, options);
        if (ltype == "autocomplete")
            options.autocomplete = true;
        if (jinput.is("select"))
        {
            jinput.ligerComboBox(options);
        }
        else if (jinput.is(":password"))
        {
            jinput.ligerTextBox(options);
        }
        else if (jinput.is(":text"))
        { 
            switch (ltype)
            {
                case "select":
                case "combobox":
                case "autocomplete": 
                    jinput.ligerComboBox(options);
                    break;
                case "spinner":
                    jinput.ligerSpinner(options);
                    break;
                case "date":
                    jinput.ligerDateEditor(options);
                    break;
                case "popup":
                    jinput.ligerPopupEdit(options);
                    break;
                case "currency":
                    options.currency = true;
                case "float":
                case "number":
                    options.number = true;
                    jinput.ligerTextBox(options);
                    break;
                case "int":
                case "digits":
                    options.digits = true;
                default:
                    jinput.ligerTextBox(options);
                    break;
            }
        }
        else if (jinput.is(":hidden"))
        { 
            //只读状态，显示文本框的形式
            if (options.readonly)
            { 
                if (field.textField)
                { 
                    var textInput = $("<input type='text' name='" + field.textField + "' />").insertAfter(jinput);
                    if (p.appendID)
                        textInput.attr("id", field.textField);
                    textInput.ligerTextBox(options);
                }
            }
            else
            {
                if ($.inArray(ltype, ["select", "combobox", "autocomplete", "popup", "radiolist", "checkboxlist", "listbox"]) != -1)
                {
                    if (!jinput.attr("id")) jinput.attr("id", liger.getId('hidden'));
                    options.valueFieldID = jinput.attr("id");
                }
                switch (ltype)
                {
                    case "select":
                    case "combobox":
                    case "autocomplete":
                    case "popup":
                        var textField = field.textField || field.comboboxName || liger.getId();
                        var textInput = $("[name='" + textField + "']", g.element);
                        if (!textInput.length)
                            textInput = $("<input type='text' name='" + textField + "' />").insertAfter(jinput);
                        if (p.appendID)
                            textInput.attr("id", textField);
                        if(ltype == "popup")
                            textInput.ligerPopupEdit(options);
                        else
                            textInput.ligerComboBox(options);
                        break;
                    case "checkboxlist":
                        $("<div></div>").insertAfter(jinput).ligerCheckBoxList(options);
                        break;
                    case "radiolist":
                        $("<div></div>").insertAfter(jinput).ligerRadioList(options);
                        break;
                    case "listbox":
                        $("<div></div>").insertAfter(jinput).ligerListBox(options);
                        break;
                }
            } 
        }
        else if (jinput.is(":radio"))
        {
            jinput.ligerRadio(options);
        }
        else if (jinput.is(":checkbox"))
        {
            jinput.ligerCheckBox(options);
        }
        else if (jinput.is("textarea"))
        {
            jinput.addClass("l-textarea");
        }
    }

    //表单组件
    $.ligerui.controls.Form = function (element, options)
    {
        $.ligerui.controls.Form.base.constructor.call(this, element, options);
    };

    $.ligerui.controls.Form.ligerExtend($.ligerui.core.UIComponent, {
        __getType: function ()
        {
            return 'Form'
        },
        __idPrev: function ()
        {
            return 'Form';
        },
        _init: function ()
        {
            $.ligerui.controls.Form.base._init.call(this);
        },
        getField: function (index)
        {
            var g = this, p = this.options;
            if (!p.fields) return null;
            return p.fields[index];
        },
        toConditions: function ()
        {
            return $.ligerui.getConditions(this.element);
        },
        //预处理字段 , 排序和分组
        _preSetFields: function (fields)
        {
            var g = this, p = this.options, lastVisitedGroup = null, lastVisitedGroupIcon = null;
            //分组： 先填充没有设置分组的字段，然后按照分组排序
            $(p.fields).each(function (i, field)
            {
                if (field.type == "hidden") return;
                if (field.newline == null) field.newline = true;
                if (lastVisitedGroup && !field.group)
                {
                    field.group = lastVisitedGroup;
                    field.groupicon = lastVisitedGroupIcon;
                }
                if (field.group)
                {
                    //trim
                    field.group = field.group.toString().replace(/^\s\s*/, '' ).replace(/\s\s*$/, '' );
                    lastVisitedGroup = field.group;
                    lastVisitedGroupIcon = field.groupicon;
                }
            }); 
           
        },
        _setFields: function (fields)
        {
            var g = this, p = this.options;
            var jform = $(this.element);
            $(".l-form-container", jform).remove();
            //自动创建表单
            if (fields && fields.length)
            {
                g._preSetFields(fields);
                if (!jform.hasClass("l-form"))
                    jform.addClass("l-form");
                var out = ['<div class="l-form-container">'];
                var appendULStartTag = false, lastVisitedGroup = null;
                var groups = [];
                $(fields).each(function (index, field)
                {
                    if ($.inArray(field.group, groups) == -1)
                        groups.push(field.group);
                }); 
                $(groups).each(function (groupIndex, group)
                {
                    $(fields).each(function (i, field)
                    {
                        if (field.group != group) return;
                        var index = $.inArray(field, fields);
                        var name = field.name || field.id, newline = field.newline;
                        if (!name) return;
                        if (field.type == "hidden")
                        {
                            out.push('<input type="hidden" id="' + name + '" name="' + name + '" />');
                            return;
                        }
                        var toAppendGroupRow = field.group && field.group != lastVisitedGroup;
                        if (index == 0 || toAppendGroupRow) newline = true;
                        if (newline)
                        {
                            if (appendULStartTag)
                            {
                                out.push('</ul>');
                                appendULStartTag = false;
                            }
                            if (toAppendGroupRow)
                            {
                                out.push('<div class="l-group');
                                if (field.groupicon)
                                    out.push(' l-group-hasicon');
                                out.push('">');
                                if (field.groupicon)
                                    out.push('<img src="' + field.groupicon + '" />');
                                out.push('<span>' + field.group + '</span></div>');
                                lastVisitedGroup = field.group;
                            }
                            out.push('<ul>');
                            appendULStartTag = true;
                        }
                        out.push('<li class="l-fieldcontainer');
                        if (newline)
                        {
                            out.push(' l-fieldcontainer-first');
                        }
                        out.push('"');
                        out.push(' fieldindex=' + index);
                        out.push('><ul>');
                        //append label
                        out.push(g._buliderLabelContainer(field, index));
                        //append input 
                        out.push(g._buliderControlContainer(field, index));
                        //append space
                        out.push(g._buliderSpaceContainer(field, index));
                        out.push('</ul></li>');

                    });
                }); 
                if (appendULStartTag)
                {
                    out.push('</ul>');
                    appendULStartTag = false;
                }
                out.push('</div>');
                jform.append(out.join(''));

                $(".l-group .togglebtn", jform).remove(); 
                $(".l-group", jform).width(jform.width() * 0.95).append("<div class='togglebtn'></div>");
            }
            //生成ligerui表单样式
            $(".l-form-container", jform).find("input,select,textarea").each(function ()
            {
                p.editorBulider.call(g, $(this));
            });
            g.trigger('afterSetFields');
        },
        _render: function ()
        {
            var g = this, p = this.options;
            var jform = $(this.element);
            //生成ligerui表单样式
            $("input,select,textarea", jform).each(function ()
            {
                p.editorBulider.call(g, $(this));
            });
            g.set(p);
            if (p.buttons)
            {
                var jbuttons = $('<ul class="l-form-buttons"></ul>').appendTo(jform);
                $(p.buttons).each(function ()
                {
                    var jbutton = $('<li><div></div></li>').appendTo(jbuttons);
                    $("div:first", jbutton).ligerButton(this);
                });
            }
        },
        //标签部分
        _buliderLabelContainer: function (field)
        {
            var g = this, p = this.options;
            var label = field.label || field.display;
            var labelWidth = field.labelWidth || field.labelwidth || p.labelWidth;
            var labelAlign = field.labelAlign || p.labelAlign;
            if (label) label += p.rightToken;
            var out = [];
            out.push('<li');
            if (p.labelCss)
            {
                out.push(' class="' + p.labelCss + '"');
            }
            out.push(' style="');
            if (labelWidth)
            {
                out.push('width:' + labelWidth + 'px;');
            }
            if (labelAlign)
            {
                out.push('text-align:' + labelAlign + ';');
            }

            out.push('">');
            if (label)
            {
                out.push(label);
            }
            out.push('</li>');
            return out.join('');
        },
        //控件部分
        _buliderControlContainer: function (field, fieldIndex)
        {
            var g = this, p = this.options;
            var width = field.width || p.inputWidth;
            var align = field.align || field.textAlign || field.textalign || p.align;
            var out = [];
            out.push('<li');
            if (p.fieldCss)
            {
                out.push(' class="' + p.fieldCss + '"');
            }
            out.push(' style="');
            if (width)
            {
                out.push('width:' + width + 'px;');
            }
            if (align)
            {
                out.push('text-align:' + align + ';');
            }
            out.push('">');
            out.push(g._buliderControl(field, fieldIndex));
            out.push('</li>');
            return out.join('');
        },
        //间隔部分
        _buliderSpaceContainer: function (field)
        {
            var g = this, p = this.options;
            var spaceWidth = field.space || field.spaceWidth || p.space;
            var out = [];
            out.push('<li');
            if (p.spaceCss)
            {
                out.push(' class="' + p.spaceCss + '"');
            }
            out.push(' style="');
            if (spaceWidth)
            {
                out.push('width:' + spaceWidth + 'px;');
            }
            out.push('">'); 
            if (field.validate && field.validate.required)
            {
                out.push("<span class='l-star'>*</span>");
            }
            out.push('</li>');
            return out.join('');
        },
        _buliderControl: function (field, fieldIndex)
        {
            var g = this, p = this.options;
            var width = field.width || p.inputWidth,
            name = field.name || field.id,
            type = (field.type || "text").toLowerCase(),
            readonly = (field.readonly || (field.editor && field.editor.readonly)) ? true : false;
            var out = [];   
            if (type == "textarea" || type == "htmleditor")
            { 
                out.push('<textarea '); 
            }
            else if ($.inArray(type, ["checkbox", "radio", "password", "file"]) != -1)
            {
                out.push('<input type="' + type + '" ');
            }
            else if ($.inArray(type, ["select", "combobox", "autocomplete", "popup", "radiolist", "checkboxlist", "listbox"]) != -1)
            {
                out.push('<input type="hidden" ');
            }
            else
            {
                out.push('<input type="text" ');
            }
            out.push('name="' + name + '" ');
            out.push('fieldindex="' + fieldIndex + '" '); 
            field.cssClass && out.push('class="' + field.cssClass + '" ');
            p.appendID  && out.push(' id="' + name + '" '); 
            out.push(g._getInputAttrHtml(field)); 
            if (field.validate && !readonly)
            {
                out.push(" validate='" + p.toJSON(field.validate) + "' ");
                g.validate = g.validate || {};
                g.validate.rules = g.validate.rules || {};
                g.validate.rules[name] = field.validate;
                if (field.validateMessage)
                {
                    g.validate.messages = g.validate.messages || {};
                    g.validate.messages[name] = field.validateMessage;
                }
            }
            out.push(' />');
            return out.join('');
        },
        _getInputAttrHtml: function (field)
        {
            var out = [], type = (field.type || "text").toLowerCase();
            if (type == "textarea")
            {
                field.cols && out.push('cols="' + field.cols + '" ');
                field.rows && out.push('rows="' + field.rows + '" ');
            }
            out.push('ltype="' + type + '" ');
            field.op && out.push('op="' + field.op + '" ');
            field.vt && out.push('vt="' + field.vt + '" ');
            if (field.attr)
            {
                for (var attrp in field.attr)
                {
                    out.push(attrp + '="' + field.attr[attrp] + '" ');
                }
            }
            return out.join('');
        }
    });
      
    //分组 收缩/展开
    $(".l-form .l-group .togglebtn").live('click', function ()
    {
        if ($(this).hasClass("togglebtn-down")) $(this).removeClass("togglebtn-down");
        else $(this).addClass("togglebtn-down");
        var boxs = $(this).parent().nextAll("ul,div");
        for (var i = 0; i < boxs.length; i++)
        {
            var jbox = $(boxs[i]);
            if (jbox.hasClass("l-group")) break;
            jbox.toggle();
        }
    });
})(jQuery);