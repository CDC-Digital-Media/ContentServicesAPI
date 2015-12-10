"use strict";

(function ($) {
    var PLUGIN_NAME = 'synd';

    // plugin signature ///////////////////////
    $[PLUGIN_NAME] = {
        defaults: {
            mediaid: '',
            apiroot: '',
            clsids: 'syndicate',
            elemids: '', 
            xpath: '',             
            stripscript : true,
            stripanchor : false,
            stripimage : false,
            stripcomment : true,
            stripstyle : true,
            imageplacement : 'none',
            oe : 'UTF-8',
            of : 'XHTML',
            ns : '',
            nw : true,
            w: '',
            h: ''
        }
    };

    // funtion //////////////////////////
    $.fn[PLUGIN_NAME] = function (options) {

        $[PLUGIN_NAME].defaults.target = this;
        options = $.extend({}, $[PLUGIN_NAME].defaults, options);

        main();

        function main() { 

            if(options.mediaid == ''){ alert('No Media Id was specified.'); return; }            

            var url = options.apiroot + '/v1/resources/media/'+ options.mediaid + '/syndicate?';

            //set values for anything not set as default -
            if(options.clsids != 'syndicate')
                {url += '&clsids=' + options.clsids;}
            if(options.elemids != '')
                {url += '&elemids=' + options.elemids;}
            if(options.xpath != '')
                {url += '&xpath=' + options.xpath;}
            if(options.stripscript != true)
                {url += '&stripscript=' + options.stripscript;}
            if(options.stripanchor != false)
                {url += '&stripanchor=' + options.stripanchor;} 
            if(options.stripimage != false)
                {url += '&stripimage=' + options.stripimage;}
            if(options.stripcomment != true)
                {url += '&stripcomment=' + options.stripcomment;}
            if(options.stripstyle != true)
                {url += '&stripstyle=' + options.stripstyle;}
            if (options.imageplacement != 'none')
                { url += '&imageplacement=' + options.imageplacement; }
            if (options.oe != 'UTF-8')
                { url += '&oe=' + options.oe; }
            if (options.of != 'XHTML')
                { url += '&of=' + options.of; }
            if (options.ns != 'cdc')
                { url += '&ns=' + options.ns; }
            if (options.nw != true)
                { url += '&nw=' + options.nw; }                          
            if (options.w != '')
                { url += '&w=' + options.w; }  
            if (options.h != '')
                { url += '&h=' + options.h; } 

            url += '&callback=?';                

            $.ajax({
                url: url,
                dataType: 'jsonp'
            })
            .done(function (response) {
                $(options.target).addClass(options.clsids)
                //var decoded = $("<div/>").html(response.results.content).text();
                $(options.target).empty().html(response.results.content);
            })
            .fail(function (xhr, ajaxOptions, thrownError) { /*alert(xhr.status); alert(thrownError);*/ })
        }
    };

})(jQuery);
