"use strict";

(function ($) {
    var PLUGIN_NAME = 'synd';

    // plugin signature ///////////////////////
    $[PLUGIN_NAME] = {
        defaults: {
            mediaid: '',
            apiroot: '',
            cssclasses: 'syndicate',
            ids: '', 
            xpath: '',             
            stripscripts : true,
            stripanchors : false,
            stripimages : false,
            stripcomments : true,
            stripstyles : true,
            imagefloat : 'none',
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

            var url = options.apiroot + '/v2/resources/media/'+ options.mediaid + '/syndicate?';

            //set values for anything not set as default -
            if(options.cssclasses != 'syndicate')
                {url += '&cssclasses=' + options.cssclasses;}
            if(options.ids != '')
                {url += '&ids=' + options.ids;}
            if(options.xpath != '')
                {url += '&xpath=' + options.xpath;}
            if(options.stripscripts != true)
                {url += '&stripscripts=' + options.stripscripts;}
            if(options.stripanchors != false)
                {url += '&stripanchors=' + options.stripanchors;} 
            if(options.stripimages != false)
                {url += '&stripimages=' + options.stripimages;}
            if(options.stripcomments != true)
                {url += '&stripcomments=' + options.stripcomments;}
            if(options.stripstyles != true)
                {url += '&stripstyles=' + options.stripstyles;}
            if (options.imagefloat != 'none')
                { url += '&imagefloat=' + options.imagefloat; }
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
                $(options.target).addClass(options.cssclasses)
                //var decoded = $("<div/>").html(response.results.content).text();
                $(options.target).empty().html(response.results.content);
            })
            .fail(function (xhr, ajaxOptions, thrownError) { /*alert(xhr.status); alert(thrownError);*/ })
        }
    };

})(jQuery);
