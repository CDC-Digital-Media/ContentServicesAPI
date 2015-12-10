"use strict";

(function (window) {
    //define parameter object
    var options = {
        mediatype: "",
        mediaid: "",
        apiroot: "",
        cssclasses: "syndicate",
        ids: "", 
        xpath: "",             
        stripscripts : true,
        stripanchors : false,
        stripimages : false,
        stripcomments : true,
        stripstyles : true,
        imagefloat : "none",
        oe : "UTF-8",
        of : "XHTML",
        ns : "",
        nw : true,
        w: "",
        h: ""
    };

    // check if the jQuery object exist
    if (typeof jQuery == "undefined") {
        // load jquery
        loadScript("//.....[productionToolsServer]...../api/embed/html/js/jquery-1.9.1.min.js", "", function () {             
            main();
        });
    }
    else {
        // jquery already loaded
        main();
    }            
    
    // main function
    function main() {            
        
        $("div[class^=rid_]:not(.syndicate)").each(function () {
           //set options
            SetOptions($(this));
            
            if (options.mediatype == "ecard") { 
                loadStyle(options.apiroot + "/embed/html/css/csEcard.css", function () {
                    loadScript(options.apiroot + "/embed/html/js/libs/CDC.Video.js", "", function () {             
                        loadScript(options.apiroot + "/embed/html/js/libs/json.js", "", function () {  
                            loadScript(options.apiroot + "/embed/html/js/libs/modernizr.js", "", function () {             
                                loadScript(options.apiroot + "/embed/html/js/libs/jquery.watermark.min.js", "", function () {             
                                    loadScript(options.apiroot + "/embed/html/js/plugins/cdc.sf.ecard.js", "", function () { 
                                        //alert("finish loading ecard"); 
                                    });
                                }); 
                            });                                   
                        });
                    });                
                });
            } else {
                BuildUrl($(this));
            }
            //
        });

    }

    function SetOptions($div) {        
        if ($div.attr("data-mediatype") && $div.attr("data-mediatype") !== "") {
            options.mediatype = $div.attr("data-mediatype");
        }
        
        if ($div.attr("data-mediaid") && $div.attr("data-mediaid") !== "") {
            options.mediaid = $div.attr("data-mediaid");
        }        
        
        if ($div.attr("data-apiroot") && $div.attr("data-apiroot") !== "") {
            options.apiroot = $div.attr("data-apiroot");
        }
        
        if ($div.attr("data-cssclasses") && $div.attr("data-cssclasses") !== "") {
            options.cssclasses = $div.attr("data-cssclasses");
        }
        
        if ($div.attr("data-ids") && $div.attr("data-ids") !== "") {
            options.ids = $div.attr("data-ids");
        }
        
        if ($div.attr("data-xpath") && $div.attr("data-xpath") !== "") {
            options.xpath = $div.attr("data-xpath");
        }
        
        if ($div.attr("data-stripscripts") && $div.attr("data-stripscripts") !== "") {
            options.stripscripts = $div.attr("data-stripscripts");
        }

        if ($div.attr("data-stripanchors") && $div.attr("data-stripanchors") !== "") {
            options.stripanchors = $div.attr("data-stripanchors");
        }
        
        if ($div.attr("data-stripimages") && $div.attr("data-stripimages") !== "") {
            options.stripimages = $div.attr("data-stripimages");
        }

        if ($div.attr("data-stripcomments") && $div.attr("data-stripcomments") !== "") {
            options.stripcomments = $div.attr("data-stripcomments");
        }
        
        if ($div.attr("data-stripstyles") && $div.attr("data-stripstyles") !== "") {
            options.stripstyles = $div.attr("data-stripstyles");
        }
        
        if ($div.attr("data-imagefloat") && $div.attr("data-imagefloat") !== "") {
            options.imagefloat = $div.attr("data-imagefloat");
        }
        
        if ($div.attr("data-oe") && $div.attr("data-oe") !== "") {
            options.oe = $div.attr("data-oe");
        }
        
        if ($div.attr("data-of") && $div.attr("data-of") !== "") {
            options.of = $div.attr("data-of");
        }
        
        if ($div.attr("data-ns") && $div.attr("data-ns") !== "") {
            options.ns = $div.attr("data-ns");
        }
        
        if ($div.attr("data-nw") && $div.attr("data-nw") !== "") {
            options.nw = $div.attr("data-nw");
        }
        
        if ($div.attr("data-w") && $div.attr("data-w") !== "") {
            options.w = $div.attr("data-w");
        }
        
        if ($div.attr("data-h") && $div.attr("data-h") !== "") {
            options.h = $div.attr("data-h");
        }                
    }

    //function definition
    function loadScript(url, id, callback) {
        var script = document.createElement("script")
        script.type = "text/javascript";

        if (id.length) { script.id = id; }
        if (script.readyState) { //IE
            script.onreadystatechange = function () {
                if (script.readyState == "loaded" || script.readyState == "complete") {
                    script.onreadystatechange = null;
                    callback();
                }
            };
        } else { //Others
            script.onload = function () {
                callback();
            };
        }
        script.src = url;
        window.document.getElementsByTagName("head")[0].appendChild(script);
    }        
    
    //function definition
    function loadStyle(url, callback) {
        var style = document.createElement("link")

        if (style.readyState) { //IE
            style.onreadystatechange = function () {
                if (style.readyState == "loaded" || style.readyState == "complete") {
                    style.onreadystatechange = null;
                    callback();
                }
            };
        } else { //Others
            style.onload = function () {
                callback();
            };
        }
        style.rel = "stylesheet";
        style.href = url;
        window.document.getElementsByTagName("head")[0].appendChild(style);
    }

    //function definition
    function BuildUrl($obj) { 
            
        if (options.mediaid == "") { alert("No Media Id was specified."); return; }            
        
        var url = options.apiroot + "/v2/resources/media/"+ options.mediaid + "/syndicate?";

        //set values for anything not set as default -
        if(options.cssclasses != "syndicate")
            {url += "&cssclasses=" + options.cssclasses;}
        if(options.ids != "")
            {url += "&ids=" + options.ids;}
        if(options.xpath != "")
            {url += "&xpath=" + options.xpath;}
            if(options.stripscripts != true)
                {url += "&stripscripts=" + options.stripscripts;}
        if(options.stripanchors != false)
            {url += "&stripanchors=" + options.stripanchors;} 
        if(options.stripimages != false)
            {url += "&stripimages=" + options.stripimages;}
            if(options.stripcomments != true)
                {url += "&stripcomments=" + options.stripcomments;}
        if(options.stripstyles != true)
            {url += "&stripstyles=" + options.stripstyles;}
        if (options.imagefloat != "none")
            { url += "&imagefloat=" + options.imagefloat; }
        if (options.oe != "UTF-8")
            { url += "&oe=" + options.oe; }
        if (options.of != "XHTML")
            { url += "&of=" + options.of; }
        if (options.ns != "cdc")
            { url += "&ns=" + options.ns; }
        if (options.nw != true)
            { url += "&nw=" + options.nw; }                          
        if (options.w != "")
            { url += "&w=" + options.w; }  
        if (options.h != "")
            { url += "&h=" + options.h; } 

        url += "&callback=?";                

        //add class
        $($obj).addClass(options.cssclasses)

        //web request
        MakeAsyncRequest(url, $obj);
    }

    function MakeAsyncRequest(url, $obj) {
        $.ajax({
            url: url,
            dataType: 'jsonp'
        })
        .done(function (response) {                        
            //var decoded = $("<div/>").html(response.results.content).text();
            $($obj).empty().html(response.results.content);
        })
        .fail(function (xhr, ajaxOptions, thrownError) { /*alert(xhr.status); alert(thrownError);*/ })        
    }
    
})(window);
