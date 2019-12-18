/**
* @requires jquery.validate.js
* @author ZhangHuihua@msn.com
*/
(function ($) {
    if ($.validator) {
        $.validator.addMethod("alphanumeric", function (value, element) {
            return this.optional(element) || /^\w+$/i.test(value);
        }, "字母、数字或下划线");

        $.validator.addMethod("lettersonly", function (value, element) {
            return this.optional(element) || /^[a-z]+$/i.test(value);
        }, "Letters only please");

        $.validator.addMethod("phone", function (value, element) {
            return this.optional(element) || /^[0-9 \(\)]{7,30}$/.test(value);
        }, "请输入一个有效的电话号码");

        $.validator.addMethod("postcode", function (value, element) {
            return this.optional(element) || /^[0-9 A-Za-z]{5,20}$/.test(value);
        }, "请输入一个有效的邮政编码");

        $.validator.addMethod("date", function (value, element) {
            value = value.replace(/\s+/g, "");
            if (String.prototype.parseDate) {
                var $input = $(element);
                var pattern = $input.attr('dateFmt') || 'yyyy-MM-dd';

                return !$input.val() || $input.val().parseDate(pattern);
            } else {
                return this.optional(element) || value.match(/^\d{4}[\/-]\d{1,2}[\/-]\d{1,2}$/);
            }
        }, "请输入一个有效的日期");

        /*自定义js函数验证
        * <input type="text" name="xxx" customvalid="xxxFn(element)" title="xxx" />
        */
        $.validator.addMethod("customvalid", function (value, element, params) {
            try {
                return eval('(' + params + ')');
            } catch (e) {
                return false;
            }
        }, "Please fix this field.");

        $.validator.addMethod("remotedwz", function (value, element, param) {
            if (this.optional(element)) {
                return "dependency-mismatch";
            }

            var previous = this.previousValue(element);
            if (!this.settings.messages[element.name]) {
                this.settings.messages[element.name] = {};
            }
            previous.originalMessage = this.settings.messages[element.name].remote;
            this.settings.messages[element.name].remote = previous.message;

            param = typeof param === "string" && { url: param} || param;

            if (this.pending[element.name]) {
                return "pending";
            }
            if (previous.old === value) {
                return previous.valid;
            }

            previous.old = value;
            var validator = this;
            this.startRequest(element);
            var data = {};
            data[element.name] = value;
            $.ajaxSettings.global = false;
            $.ajax($.extend(true, {
                url: param,
                mode: "abort",
                port: "validate" + element.name,
                dataType: "json",
                data: data,
                success: function (response) {
                    validator.settings.messages[element.name].remote = previous.originalMessage;
                    var valid = response === true || response === "true";
                    if (valid) {
                        var submitted = validator.formSubmitted;
                        validator.prepareElement(element);
                        validator.formSubmitted = submitted;
                        validator.successList.push(element);
                        delete validator.invalid[element.name];
                        validator.showErrors();
                    } else {
                        var errors = {};
                        var message = response || validator.defaultMessage(element, "remote");
                        errors[element.name] = previous.message = $.isFunction(message) ? message(value) : message;
                        validator.invalid[element.name] = true;
                        validator.showErrors(errors);
                    }
                    previous.valid = valid;
                    validator.stopRequest(element, valid);
                }
            }, param));
            $.ajaxSettings.global = true;
            return "pending";
        }, "请修正该字段");

        $.validator.addClassRules({
            date: { date: true },
            alphanumeric: { alphanumeric: true },
            lettersonly: { lettersonly: true },
            phone: { phone: true },
            postcode: { postcode: true }
        });
        $.validator.setDefaults({ errorElement: "span" });
        $.validator.autoCreateRanges = true;

    }

})(jQuery);