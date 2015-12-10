"use strict"; //ignore jslint

(function ($) {
    // plugin signature ///////////////////////
    var options = {
        mediaid: "",
        apiroot: "",
        cssclass: "csEcard",
        filepath: "",
        navigationtext: "Choose another eCard",
        navigationurl: "",
        completenavigationtext: "Choose another eCard",
        completenavigationurl: "",
        receipturl: ""
    };

    function SetOptions($div) {
        if ($div.attr("data-mediaid") && $div.attr("data-mediaid") !== "") {
            options.mediaid = $div.attr("data-mediaid");
        }     

        if ($div.attr("data-apiroot") && $div.attr("data-apiroot") !== "") {
            options.apiroot = $div.attr("data-apiroot");
        }             
                    
        if ($div.attr("data-cssclass") && $div.attr("data-cssclass") !== "") {
            options.cssclass = $div.attr("data-cssclass");
        }

        if ($div.attr("data-filepath") && $div.attr("data-filepath") !== "") {
            options.filepath = $div.attr("data-filepath");
        }

        if ($div.attr("data-navigationtext") && $div.attr("data-navigationtext") !== "") {
            options.navigationtext = $div.attr("data-navigationtext");
        }

        if ($div.attr("data-navigationurl") && $div.attr("data-navigationurl") !== "") {
            options.navigationurl = $div.attr("data-navigationurl");
        }

        if ($div.attr("data-completenavigationtext") && $div.attr("data-completenavigationtext") !== "") {
            options.completenavigationtext = $div.attr("data-completenavigationtext");
        }

        if ($div.attr("data-completenavigationurl") && $div.attr("data-completenavigationurl") !== "") {
            options.completenavigationurl = $div.attr("data-completenavigationurl");
        }

        if ($div.attr("data-receipturl") && $div.attr("data-receipturl") !== "") {
            options.receipturl = $div.attr("data-receipturl");
        }                
    }

    var disclaimerSender = "<p>DISCLAIMER</p><p>CDC's Health-e-Cards allow the public to send electronic greeting cards to friends, family, and co-workers. To personalize this service, you are allowed to send personal messages with a limit of 500 characters or less.</p><p>When using the personal message feature in CDC's Health-e-Cards, you are responsible for the content of the message. Please note:</p><ul><li>Comments and views expressed in the personal message feature are those of the individual sending the response and do not necessarily reflect those of the CDC/HHS or the Federal government.</li><li>CDC/HHS does not control or guarantee the accuracy, relevance, timeliness or completeness of the information sent in a personal message.</li><li>CDC/HHS does not endorse content or links provided in a personal message.</li><li>CDC/HHS does not authorize the use of copyrighted materials contained in a personal message. Those who provide comments are responsible for the copyright of the text they provide.</li><li>CDC's Health-e-Cards follow the .....[domain]..... privacy policy. CDC will not share or sell any personal information obtained from users with any other organization or government agency except as required by law. Please view all of our policies and regulations at <a href='http://www......[domain]...../Other/policies.html'>http://www......[domain]...../Other/policies.html.</a></li></ul>";
    var disclaimerRecipient = "<p>DISCLAIMER</p><p>Comments and views expressed in the personal message feature are those of the individual sending the personal message and do not necessarily reflect those of the Centers for Disease Control and Prevention (CDC), the Department of Health and Human Services (DHHS) or the Federal government. CDC/DHHS does not control or guarantee the accuracy or relevance of the information sent in a personal message, nor does CDC/DHHS endorse any content or links provided therein. To report abuse please send an email to <a href='senderEmail1@email'>senderEmail1@email</a>";
    var successMsg = "<p class='csEcard_successHeader'>Your eCard was successfully sent!</p><p>Thank you for using CDC's Health-e-Card system and helping us with our mission of safer, healthier people.</p>";


    ////////////////////////////
    (function (options) {
            
        $("div[class^=rid_]:not(.syndicate)[data-mediatype='ecard']").each(function () {
           //set options
            SetOptions($(this));

            var ecard = {};

            if (options.receipturl === "") {
                options.receipturl = document.location;
            }

            var htmlEscape = function (str) {
                return String(str)
                        .replace(/&/g, '&amp;')
                        .replace(/"/g, '&quot;')
                        .replace(/'/g, '&#39;')
                        .replace(/</g, '&lt;')
                        .replace(/>/g, '&gt;');
            };

            /// make ajax call - on success build out card (main())
            MakeAsyncRequest($(this), ecard);

        });

        function MakeAsyncRequest($elem, ecard) {
            /// make ajax call - on success build out card (main())
            $.ajax({
                url: options.apiroot + '/v2/resources/media/' + options.mediaid,
                dataType: 'jsonp'
            })
                .done(function (response) {
                    if (response.results) {
                        ecard = response.results[0];
                        main($elem, ecard);
                    }
                    else {
                        return;
                    }
                })
                .fail(function (xhr, ajaxOptions, thrownError) { /*alert(xhr.status); alert(thrownError);*/ });
        }

        var $root;
        function buildEcard($elem) {
            $($elem).children().remove();

            $root = $("<div class='" + options.cssclass + "'>");
            $($elem).append($root);

            var $cardContainer = $("<div class='csEcard_card' id='cardDiv'>");
            $root.append($cardContainer);

            var $mailForm = $("<div class='csEcard_form'>");
            $root.append($mailForm);

            var $requiredNote = $("<div class='csEcard_requiredNote'>").append("<span class='csEcard_warning'>*</span> Required field | <a href='#' class='csEcard_viewFront'>View front of Card</a>");

            $mailForm.append($requiredNote);

            var $recipients = $("<div class='csEcard_recipients'>")
                                .append("<div>Send this eCard to</div>");
            $mailForm.append($recipients);

            var $recipient = $("<div class='csEcard_recipient'>")
                                .append("<input type='text' class='csEcard_recipName' placeholder='Recipient Name'/>")
                                .append("<span class='csEcard_warning'>*</span>")
                                .append("<input type='text' class='csEcard_recipEmail' placeholder='Email Address'/>")
                                .append("<span class='csEcard_warning'>*</span>");

            $recipients.append($recipient);

            var $addRecipient = $("<div class='csEcard_addRecipient'>")
                                .append("Would you like to <a href='#' class='csEcard_addRecipentLink'>Add another Recipient</a>?");

            $mailForm.append($addRecipient);

            var $msg = $("<div class='csEcard_message'>")
                                .append("<div class='csEcard_msgHeader'>Personal Message</div>")
                                .append("<textarea class='csEcard_msgArea' rows='4'></textarea>")
                                .append("<div class='csEcard_msgWarning'>Please limit the message to 500 characters or less and special characters like &lt;&gt; are not accepted.</div>");

            $mailForm.append($msg);

            var $sender = $("<div class='csEcard_sender'>")
                                .append("<div>From</div>")
                                .append("<input type='text' class='csEcard_senderName' placeholder='Your Name'/>")
                                .append("<span class='csEcard_warning'>*</span>")
                                .append("<input type='text' class='csEcard_senderEmail' placeholder='Email Address'/>")
                                .append("<span class='csEcard_warning'>*</span>");

            $mailForm.append($sender);

            var $disclaimer = $("<div class='csEcard_disclaimer'>").html(disclaimerSender);
            $mailForm.append($disclaimer);

            var $success = $("<div class='csEcard_success'>").html(successMsg);
            $mailForm.append($success);

            var $navigation = $("<div class='csEcard_navigation'>")
                                .append("<a class='csEcard_btn csEcard_preview' href='#'>Preview this eCard</a>")
                                .append("<a class='csEcard_btn csEcard_edit' href='#'>Edit this eCard</a>")
                                .append("<a class='csEcard_btn csEcard_send' href='#'>Send this eCard</a>")
                                .append("<div class='csEcard_copySelf'><input type='checkbox' class='csEcard_chkCopySelf'> Send me a copy of the email</div>")
                                .append("<a class='csEcard_btn csEcard_restart' href='#'>Send this eCard to someone else.</a>");

            if (options.navigationtext !== "" && options.navigationurl !== "") {
                $navigation.append("<a class='csEcard_btn csEcard_return' href='" + options.navigationurl + "'>" + options.navigationtext + "</a>");
            }

            if (options.completenavigationtext !== "" && options.completenavigationurl !== "") {
                $navigation.append("<a class='csEcard_btn csEcard_complete' href='" + options.completenavigationurl + "'>" + options.completenavigationtext + "</a>");
            }

            $mailForm.append($navigation);

            $root.find(".csEcard_success").hide();
            $root.find(".csEcard_edit").hide();
            $root.find(".csEcard_copySelf").hide();
            $root.find(".csEcard_send").hide();
            $root.find(".csEcard_restart").hide();
            $root.find(".csEcard_complete").hide();

        }

        function attachEvents(ecard) {
            applyWatermark();

            $root.find('.csEcard_viewFront').click(function () {
                $root.find('.csEcard_card').children().remove();
                buildOutCardDisplay($root.find('.csEcard_card'), ecard);
                return false;
            });

            $root.find('.csEcard_addRecipentLink').click(function () {

                var $recRow = $root.find('.csEcard_recipient').first().clone();
                $recRow.find('.csEcard_warning').remove();
                $recRow.find('input').each(function () { $(this).val(''); });
                $root.find('.csEcard_recipients').append($recRow);

                applyWatermark();
                $root.find('.csEcard_recipName').last().focus();

                if ($root.find('.csEcard_recipient').length == 5) {
                    $root.find('.csEcard_addRecipentLink').parent('div').hide();
                }

                return false;
            });

            $root.find(".csEcard_msgArea")
                .keydown(function () { checkLength(event, this, '500'); })
                .keyup(function () { checkLength(event, this, '500'); })
                .blur(function () { checkLength(event, this, '500'); });

            $root.find('.csEcard_preview').click(function () {
                if (validateForm()) {
                    showPreview(ecard);
                    $('html, body').animate({
                        scrollTop: $root.find('.csEcard_card').offset().top
                    }, 0);
                }
                return false;
            });

            $root.find(".csEcard_edit").click(function () {
                showEdit(ecard);
            });

            $root.find(".csEcard_send").click(function () {
                sendEcard();
            });

        }

        //#region utility functions

        function sendEcard() {
            var obj = {};

            var recipName = $(".csEcard_recipName").map(function () { return htmlEscape($(this).val()); }).get();
            var recipEmail = $(".csEcard_recipEmail").map(function () { return htmlEscape($(this).val()); }).get();
            var senderName = htmlEscape($(".csEcard_senderName").val());
            var senderEmail = htmlEscape($(".csEcard_senderEmail").val());

            var arRecipient = [];

            $(recipName).each(function (index, item) {
                arRecipient.push({
                    name: recipName[index],
                    emailAddress: recipEmail[index]
                });
            });

            if ($(".csEcard_chkCopySelf").is(':checked')) {
                arRecipient.push({
                    name: senderName,
                    emailAddress: senderEmail
                });
            }

            var ecardData = {
                recipients: arRecipient,
                personalMessage: htmlEscape($(".csEcard_msgArea").val()),
                senderName: senderName,
                senderEmail: senderEmail,
                isFromMobile: false,
                eCardApplicationUrl: encodeURIComponent(options.receipturl)
            };


            $.ajax({
                type: "POST",
                url: "Secure.aspx/SendEcard",
                data: "{data: '" + JSON.stringify(ecardData) + "', apiUrl: '" + _apiroot + "/api/v1/resources/ecards/" + options.mediaid + "/send'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (sendResponse) {
                    var obj = (typeof sendResponse.d) == 'string' ? eval('(' + sendResponse.d + ')') : sendResponse.d;

                    if (obj.meta.status === 200) {
                        $root.find('.csEcard_card').hide();
                        $root.find('.csEcard_messageRcpt').hide();
                        $root.find('.csEcard_disclaimer').hide();
                        $root.find(".csEcard_edit").hide();
                        $root.find(".csEcard_copySelf").hide();
                        $root.find(".csEcard_send").hide();

                        $root.find(".csEcard_success").show();

                        $root.find(".csEcard_restart").show();
                        $root.find(".csEcard_complete").show();
                        $root.find(".csEcard_restart").click(function () {
                            main();
                        });
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    console.debug(xhr.status);
                    console.debug(thrownError);
                    console.debug(xhr.responseText);
                    alert(xhr.status);
                    alert(thrownError);
                }
            });

        }

        function showPreview(ecard) {
            //reset card
            $root.find('.csEcard_card').children().remove();
            buildOutCardDisplay($root.find('.csEcard_card'), ecard);

            $root.find('.csEcard_requiredNote').hide();
            $root.find('.csEcard_recipients').hide();
            $root.find('.csEcard_addRecipient').hide();
            $root.find('.csEcard_message').hide();
            $root.find('.csEcard_sender').hide();
            $root.find('.csEcard_disclaimer').html(disclaimerRecipient);


            var $msgRecipient = $("<div class='csEcard_messageRcpt'>")
                                .append("<div class='csEcard_msgRcptHeader'>Your friend included a personal message to you:</div>")
                                .append("<div class='csEcard_msgRcptContent'>" + $root.find(".csEcard_msgArea").val() + "</div>")
                                .append("<div class='csEcard_msgRcptSalutation'>Sincerely,</div>")
                                .append("<div class='csEcard_msgSenderName'>" + $root.find(".csEcard_senderName").val() + "</div>");


            $root.find(".csEcard_card").after($msgRecipient);

            $root.find(".csEcard_return").hide();
            $root.find(".csEcard_preview").hide();
            $root.find(".csEcard_edit").show();
            $root.find(".csEcard_copySelf").show();
            $root.find(".csEcard_send").show();

            /*
            [checkbox]Send me a copy of the email 
            */
        }

        function showEdit(ecard) {
            //reset card
            $root.find('.csEcard_card').children().remove();
            buildOutCardDisplay($root.find('.csEcard_card'), ecard);

            $root.find('.csEcard_requiredNote').show();
            $root.find('.csEcard_recipients').show();

            if ($root.find('.csEcard_recipient').length < 5) {
                $root.find('.csEcard_addRecipient').show();
            }

            $root.find('.csEcard_message').show();
            $root.find('.csEcard_sender').show();

            $root.find('.csEcard_messageRcpt').hide();

            $root.find('.csEcard_disclaimer').html(disclaimerSender);

            $root.find(".csEcard_return").show();
            $root.find(".csEcard_preview").show();
            $root.find(".csEcard_edit").hide();
            $root.find(".csEcard_send").hide();
        }



        function buildOutCardDisplay($container, ecard) {
            var doFlash = false;
            var doHtml5 = false;
            var html = "";

            if (!!document.createElement('canvas').getContext && ecard.extension.html5Source !== null && ecard.extension.html5Source !== '') {

                html += "<iframe class='html5Frame' width='580px' height='400px' scrolling='no' src='" + ecard.extension.html5Source + "'>";
                html += "</iframe>";
            }

            else if (ecard.sourceUrl !== null) {
                doFlash = true;

                html += "<div id='flashContentALT'>";
                html += "<a title='" + ecard.title + "' href='" + ecard.targetUrl + "'>";
                // need resource for alternate image rather than relying on the source imgage used for moble.
                html += "<img title='" + ecard.title + "' src='" + ecard.extension.imageSourceOutsideLarge + "' alt='" + ecard.title + "' class='cardImg'/>";
                html += "</a>";
                html += "</div>";
            }

            $container.append(html);

            if (doFlash) {// have to apply after HTML has been added to DOM
                var Myflash = new CDC.Video("", "flashContentALT", "MyEcardsButton", true,
                                    580, 400, ecard.sourceUrl,
                                    580, 400, ecard.sourceUrl,
                                    false);
                Myflash.render();
            }
        }

        function validateForm() {
            var isvalid = true;

            // verify that recipient fields are appropriately filled (matched).
            $root.find(".csEcard_recipEmail").each(function () {
                if (isvalid &&
                            $(this).val() === '' &&
                            $(this).parents('div').first().find('.csEcard_recipName').val() !== '') {
                    alert('Please enter an email address for each recipient name.');
                    isvalid = false;
                    return;
                }
            });
            $root.find(".csEcard_recipName").each(function () {
                if (isvalid &&
                            $(this).val() === '' &&
                            $(this).parents('div').first().find('.csEcard_recipEmail').val() !== '') {
                    alert('Please enter a name for each recipient email address.');
                    isvalid = false;
                    return;
                }
            });
            // verify that values in email fields are in email formats
            $root.find(".csEcard_recipEmail").each(function () {
                if (isvalid &&
                            $(this).val() !== '') {
                    if (!validateEmail($(this).val())) {
                        alert('"' + $(this).val() + '" is not a valid email address.');
                        isvalid = false;
                        return;
                    }
                }
            });

            // verify that at least one email address exists
            var empty = true;
            $root.find(".csEcard_recipEmail").each(function () {
                if ($(this).val() !== '') {
                    empty = false;
                    return;
                }
            });
            if (isvalid && empty) {
                alert('Please enter name and email of at least one recipient.');
                isvalid = false;
            }

            if (isvalid && $(".csEcard_senderName").val() === '') {
                alert('Please enter your name.');
                isvalid = false;
            }

            if (isvalid && $(".csEcard_senderEmail").val() === '') {
                alert('Please enter your email address.');
                isvalid = false;
            }

            if (isvalid && !validateEmail($root.find(".csEcard_senderEmail").val())) {
                alert('"' + $root.find(".csEcard_senderEmail").val() + '" is not a valid email address.');
                isvalid = false;
            }

            return isvalid;
        }

        function checkLength(e, oObject, maxlength) {
            if (oObject.value.length < maxlength)
                return true;
            else {

                var keyID = (window.event) ? event.keyCode : e.keyCode;
                if ((keyID >= 37 && keyID <= 40) || (keyID == 8) || (keyID == 46)) {
                    if (window.event)
                        e.returnValue = true;
                }
                else {
                    if (window.event)
                        e.returnValue = false;
                    else
                        e.preventDefault();
                }
            }
        }

        function validateEmail(email) {
            var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(email);
        }

        //#endregion

        function main($elem, ecard) {
            buildEcard($elem);
            buildOutCardDisplay($('.csEcard_card'), ecard);
            attachEvents(ecard);
        }

    })(options);

    function applyWatermark() {
        $("[placeholder]").each(function () {
            $(this).watermark($(this).attr("placeholder"));
        });
    }

})(jQuery);
