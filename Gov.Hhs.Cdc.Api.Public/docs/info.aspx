<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="info.aspx.cs" Inherits="Gov.Hhs.Cdc.Api.Public.docs.info" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <title>CS API Reference</title>
    <link rel="stylesheet" href="css/site.css" media="screen" />

    <script src="js/jquery.js"></script>
    <script src="js/jquery-cookie.js"></script>
    <script src="js/waypoints.js"></script>
    <script src="js/api.js"></script>

</head>

<body class="api-docs json mozilla">
<%--    <div id="background"></div>
    <div id="header">FFF
      <h1><a>Content Services API</a></h1>
    </div>--%>
    <div id="guide">

        <p class="ref-title">API</p>

        <ul class="ref-list">
            <li class="section">
                <a class="parent viewing" href="#intro">Introduction</a>
            </li>
            <li class="section">
                <a class="parent" href="#response">Response</a>
            <span class="children">
                <a class="child" href="#meta">Meta</a>            
            </span>
            </li>
        </ul>

        <p class="ref-title">METHODS</p>

        <ul class="ref-list">
          <!-- media (ref) -->
          <li class="section section-media">
            <a class="parent" href="#media">Media</a>
            <span class="children">
              <a class="child" href="#search_media">Search</a>
              <a class="child" href="#retrieve_media">Retrieve Media by id</a>
              <a class="child" href="#sort_media">Sort</a>
              <a class="child" href="#page_media">Pagination</a>
              <a class="child" href="#embed_media">Embed Code</a>
              <a class="child" href="#syndicate_media">Syndication</a>
              <a class="child" href="#syndicate_content">Content</a>
            </span>
          </li>

          <!-- media types (ref) -->
          <li class="section section-discovery">
            <a class="parent" href="#mediatypes">Media Types</a>
            <span class="children">
            </span>
          </li>

          <!-- topics (ref) -->
          <li class="section section-discovery">
            <a class="parent" href="#topics">Topics</a>
            <span class="children">
            </span>
          </li>

          <!-- Audiences (ref) -->
          <li class="section section-discovery">
            <a class="parent" href="#audiences">Audiences</a>
            <span class="children">
            </span>
          </li>

          <!-- tags (ref) -->
          <li class="section section-discovery">
            <a class="parent" href="#tags">Tags</a>
            <span class="children">
              <a class="child" href="#retrieve_tag">Retrieve tag by id</a>
              <a class="child" href="#retrieve_tag_media">Retrieve media by tag id</a>
              <a class="child" href="#related_tags">Related Tags</a>
            </span>
          </li>

          <!-- tagtypes (ref) -->
          <li class="section section-discovery">
            <a class="parent" href="#tagtypes">Tag Types</a>
            <span class="children">
            </span>
          </li>

          <!-- languages (ref) -->
          <li class="section section-discovery">
            <a class="parent" href="#languages">Languages</a>
            <span class="children">
            </span>
          </li>

          <!-- organizations (ref) -->
          <li class="section section-discovery">
            <a class="parent" href="#organizations">Organizations</a>
            <span class="children">
            </span>
          </li>

          <!-- organization types (ref) -->
          <li class="section section-discovery">
            <a class="parent" href="#organizationtypes">Organization Types</a>
            <span class="children">
            </span>
          </li>

          <!-- organization types (ref) -->
          <li class="section section-discovery">
            <a class="parent" href="#sources">Sources</a>
            <span class="children">
            </span>
          </li>
      </ul>

        <p class="ref-title">REFERENCES</p>

        <ul class="ref-list">
          <!-- upgrades (ref) -->
          <li class="section section-media">
            <a class="parent" href="#api_upgrades">Upgrades</a>
            <span class="children">
              <a class="child" href="#media_upgrades">Media</a>
              <a class="child" href="#syndication_upgrades">Syndication</a>
              <a class="child" href="#language_upgrades">Languages</a>
            </span>
          </li>

          <!-- change log (ref) -->
          <li class="section section-media" style="display:none;">
            <a class="parent" href="#api_change_log">Change Log</a>
            <span class="children">
            </span>
          </li>
        
        <!-- pervious version (ref) -->
          <li class="section section-media">
            <a class="parent" href="v1/info.aspx" target="_blank">Previous Version</a>
            <span class="children">
            </span>
          </li>
      </ul>
    </div>

    <!-- api docs -->
    <div id="api-docs">
      <a id="top" name="top" class="section-anchor">&nbsp;</a>
      <div id="methods">
        <div class="border"></div>

        <div id="language">
            <a class="language selected" href="javascript:void(0);">JSON</a>
<!--            <a class="language" href="#">json</a>
            <a class="language" href="#">Ruby</a>
            <a class="language" href="#">Python</a>
            <a class="language" href="#>PHP</a>
            <a class="language" href="#">Java</a>
            <a class="language" href="#">Node</a>-->
        </div>

        <div class="method">
          <a id="intro" name="intro" class="section-anchor">&nbsp;</a>
<div class="method-section">
  <div class="method-description">

    <h1>API Reference</h1>

    <p>
The Content Services API is RESTful. 
We use HTTP response codes to indicate API errors, 
and also use HTTP verbs which can be understood by off-the-shelf HTTP clients.  
JSON will be the default returned in all responses where the format is not specified, including errors.
    </p>

        <div class="method-list method-attr">
        <h6>Formats</h6>
        <p>
          The supported formats are JSON, JSONP, and XML.
        </p>

        <dl>
          <dt>Usage</dt>
          <dd><span>You can specify the format as an extension (.../media<b>.xml</b>) 
          <br />or as a querystring (...?<b>format=xml</b>).
          <br />If both formats are present, the extension format will be used.
          <br />For the extension format: Append the format (.jsonp) to the last resource before the querystring.</span></dd>
          <div class="clearfix"></div>
      </dl>

    </div>
  </div>
  <div class="method-example" id="api-summary-example">
<!--
      <h3>Libraries</h3>
      <p>Libraries are <a href="<% Response.Write(HostName);%>/docs/libraries">available in several languages</a>.</p>-->

      <h3>API Endpoint</h3>
      <code><% Response.Write(HostName);%>/api</code>

      <h6>Summary of Resource URL Patterns</h6>
      <ul>
        <li>/<% Response.Write(Version);%>/resources/media</li>
        <li>/<% Response.Write(Version);%>/resources/media?q={STRING}</li>
        <li>/<% Response.Write(Version);%>/resources/media?mediatypes={MEDIA_TYPE_NAME}</li>
        <li>/<% Response.Write(Version);%>/resources/media?name={STRING}</li>
        <li>/<% Response.Write(Version);%>/resources/media?topic={STRING}</li>
        <li>/<% Response.Write(Version);%>/resources/media?topicids={CSV}</li>
        <li>/<% Response.Write(Version);%>/resources/media?audience={STRING}</li>
        <li>/<% Response.Write(Version);%>/resources/media?tags={STRING}</li>
        <li>/<% Response.Write(Version);%>/resources/media?tagtypes={STRING}</li>
        <li>/<% Response.Write(Version);%>/resources/media?languagename={STRING}</li>
        <li>/<% Response.Write(Version);%>/resources/media?languageisocode={STRING}</li>
        <li>/<% Response.Write(Version);%>/resources/media?sourcename={STRING}</li>
        <li>/<% Response.Write(Version);%>/resources/media?sourceacronym={STRING}</li>
        <li>/<% Response.Write(Version);%>/resources/media/{ID}?showchildlevel={POSITIVE_INTEGER}</li>
        <li>/<% Response.Write(Version);%>/resources/media/{ID}?showparentlevel={POSITIVE_INTEGER}</li>
        <li>/<% Response.Write(Version);%>/resources/media?ttl={POSITIVE_INTEGER}</li>
        <li>/<% Response.Write(Version);%>/resources/media/{MEDIA_ID}</li>
        <li>/<% Response.Write(Version);%>/resources/media?sort={ATTRIBUTE_NAME}</li>
        <li>/<% Response.Write(Version);%>/resources/media?sort=—{ATTRIBUTE_NAME}</li>
        <li>/<% Response.Write(Version);%>/resources/media?sort={ATTRIBUTE_NAME}&order=ASC</li>
        <li>/<% Response.Write(Version);%>/resources/media?sort={ATTRIBUTE_NAME}&order=DESC</li>
        <li>/<% Response.Write(Version);%>/resources/media?max={POSITIVE_INTEGER}&pagenum={POSITIVE_INTEGER}</li>
        <li>/<% Response.Write(Version);%>/resources/media?max={POSITIVE_INTEGER}&offset={POSITIVE_INTEGER}</li>
        <li>/<% Response.Write(Version);%>/resources/media?max={POSITIVE_INTEGER}&offset={POSITIVE_INTEGER}&pagenum={POSITIVE_INTEGER}</li>
        <li>/<% Response.Write(Version);%>/resources/media/{MEDIA_ID}/embed</li>
        <li>/<% Response.Write(Version);%>/resources/media/{MEDIA_ID}/syndicate</li>
        <li>/<% Response.Write(Version);%>/resources/media/{MEDIA_ID}/content</li>
        <li>/<% Response.Write(Version);%>/resources/mediatypes</li>
        <li>/<% Response.Write(Version);%>/resources/topics</li>
        <li>/<% Response.Write(Version);%>/resources/audiences</li>
        <li>/<% Response.Write(Version);%>/resources/languages</li>
        <li>/<% Response.Write(Version);%>/resources/organizations</li>
        <li>/<% Response.Write(Version);%>/resources/organizationtypes</li>
        <li>/<% Response.Write(Version);%>/resources/sources</li>
      </ul>

  </div>
</div>
</div>

        <div class="method">
          <a id="response" name="response" class="section-anchor">&nbsp;</a>

<div class="method-section">
  
  <div class="method-description">
    <h3>Response</h3>
    <p class="lang-json">
        The API returns the result of ALL requests using the same data structure.
    </p>
    <p>
        All <code>DateTime</code> fields are in the <b>ISO 8601</b> format: yyyy-MM-dd<b>T</b>HH:mm:ss<b>Z</b>
    </p>
    <div class="method-list method-attr">
        <h6>Attributes</h6>
        <dl>
          <dt>meta</dt>
          <dd><span>Contains metadata about the response object.</span></dd>
          <div class="clearfix"></div>

          <dt>results</dt>
          <dd><span>Array of objects that contains the data.</span></dd>
          <div class="clearfix"></div>
      </dl>
    </div>

    <a id="meta" name="meta" class="section-anchor">&nbsp;</a>
    <div class="method-list method-attr">
        <h6>Meta</h6>
        <dl>
          <dt>status</dt>
          <dd>positive integer
        <span><b>200</b> OK - Everything worked as expected.</span>
        <span><b>400</b> Bad Request - Often missing a required parameter.</span>
        <span><b>500</b> Server errors - something went wrong on our end.</span>
    
          </dd>
          <div class="clearfix"></div>

          <dt>message</dt>
          <dd>
            array of objects <span>Used to display information about each request that is related to the status (meta).</span>
            <div class="method-list collapsed">
              <dl class="child-list">
                <a class="show-parameters">Show Child Attributes...</a>
                <dt><span class="lang lang-json">type</span></dt>
                <dd>string<span>
                    Describes the type of message (Ex: "Error").
                  </span>
                </dd>
                <div class="clearfix"></div>

                <dt><span class="lang lang-json">code</span></dt>
                <dd>string</dd>
                <div class="clearfix"></div>

                <dt><span class="lang lang-json">id</span></dt>
                <dd>string</dd>
                <div class="clearfix"></div>

                <dt><span class="lang lang-json">userMessage</span></dt>
                <dd>string
                  <span>
                    A user-friendly information giving more details about the message.
                  </span>
                </dd>
                <div class="clearfix"></div>

                <dt><span class="lang lang-json">developerMessage</span></dt>
                <dd>
                  string
                  <span>
                    A developer-friendly information giving more details about the message.
                  </span>
                </dd>
                <div class="clearfix"></div>

              </dl>
            </div>
          </dd>
          <div class="clearfix"></div>

          <dt>resultSet</dt>
          <dd><span>Information about the result.</span>
          <div class="method-list collapsed">
              <dl class="child-list">
                <a class="show-parameters">Show Child Attributes...</a>
                <dt><span class="lang lang-json">id</span></dt>
                <dd>string <em></em><span>
                    The cache identifier for the result.
                  </span>
                </dd>
                <div class="clearfix"></div>
              </dl>
            </div>
          </dd>
          <div class="clearfix"></div>

          <dt>pagination</dt>
          <dd><span>Paging information about the data returned.</span>
          <div class="method-list collapsed">
              <dl class="child-list">
                <a class="show-parameters">Show Child Attributes...</a>
                <dt><span class="lang lang-json">total</span></dt>
                <dd>positive integer<em> or zero</em></dd>
                <div class="clearfix"></div>

                <dt><span class="lang lang-json">count</span></dt>
                <dd>positive integer<em> or zero</em></dd>
                <div class="clearfix"></div>

                <dt><span class="lang lang-json">max</span></dt>
                <dd>positive integer<em> or zero</em></dd>
                <div class="clearfix"></div>

                <dt><span class="lang lang-json">offset</span></dt>
                <dd>positive integer<em> or zero</em></dd>
                <div class="clearfix"></div>

                <dt><span class="lang lang-json">pageNum</span></dt>
                <dd>positive integer<em> or zero</em></dd>
                <div class="clearfix"></div>

                <dt><span class="lang lang-json">totalPages</span></dt>
                <dd>positive integer<em> or zero</em></dd>
                <div class="clearfix"></div>

                <dt><span class="lang lang-json">sort</span></dt>
                <dd>string<em></em></dd>
                <div class="clearfix"></div>

                <dt><span class="lang lang-json">currentUrl</span></dt>
                <dd>string<em></em></dd>
                <div class="clearfix"></div>

                <dt><span class="lang lang-json">previousUrl</span></dt>
                <dd>string<em></em></dd>
                <div class="clearfix"></div>

                <dt><span class="lang lang-json">nextUrl</span></dt>
                <dd>string<em></em></dd>
                <div class="clearfix"></div>

              </dl>
            </div>
          </dd>
          <div class="clearfix"></div>
      </dl>
    </div>

  </div>

  <div class="method-example">
    <code class="method-object"><span class="lang lang-json"><span class="highlight_js bash">{
    <span class="string">"meta"</span>: {
        <span class="string">"status"</span>: 200,
        <span class="string">"message"</span>: [
          {
            <span class="string">"type"</span>: <span class="string">"Error"</span>,
            <span class="string">"code"</span>: <span class="string">""</span>,
            <span class="string">"id"</span>: <span class="string">""</span>,
            <span class="string">"userMessage"</span>: <span class="string">""</span>,
            <span class="string">"developerMessage"</span>: <span class="string">""</span>
          }
        ],
        <span class="string">"resultSet"</span>: {
          <span class="string">"id"</span>: <span class="string">"becbbe39-7308-422d-a567-690d588c4600"</span>
        },
        <span class="string">"pagination"</span>: {
          <span class="string">"total"</span>: 94,
          <span class="string">"count"</span>: 20,
          <span class="string">"max"</span>: 20,
          <span class="string">"offset"</span>: 20,
          <span class="string">"pageNum"</span>: 1,
          <span class="string">"totalPages"</span>: 5,
          <span class="string">"sort"</span>: <span class="string">"datePublished DESC, name ASC"</span>,
          <span class="string">"currentUrl"</span>: <span class="string">"<% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?mediatype=html&max=20&offset=20&pagenum=3"</span>,
          <span class="string">"previousUrl"</span>: null,
          <span class="string">"nextUrl"</span>: null
        }
    },
    <span class="string">"results"</span>: [
        {...},
        {...}
    ]
}</span>
    </span>
</code>

</div>

</div>
        </div>                                       

        <div class="method">
          <a id="A1" name="media" class="section-anchor">&nbsp;</a>
<div class="method-section">
  <div class="method-description">

    <h2>Media</h2>
    <p>
    Represents all the multi-media types (HTML, Images, Buttons, Badges, Widgets, Infographics, etc.) in the Content Services System.
    </p>

    <div class="method-list">
      <h6>Attributes</h6>
  <dl class="argument-list">
        
          <dt>id</dt>
          <dd class="">positive integer</dd>
        <div class="clearfix"></div>
        
          <dt>name</dt>
          <dd class="">string</dd>
        <div class="clearfix"></div>
          
        <dt>description</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>
                  
          <dt>mediaType</dt>
          <dd class="">string 
        </dd>
        <div class="clearfix"></div>
        
          <dt>language</dt>
          <dd class=""><a href="#languages">language</a>
        </dd>
        <div class="clearfix"></div>

          <dt>tags</dt>
          <dd>array of objects<em></em></dd>
        </dd>
        <div class="clearfix"></div> 
                
        <dt>campaigns</dt>
        <dd>array of objects<em></em></dd>
        <div class="clearfix"></div>

          <dt>source</dt>
          <dd><a href="#source">source</a>
        </dd>
        <div class="clearfix"></div>

        <dt>attribution</dt>
        <dd class="">string</dd>
        <div class="clearfix"></div>

        <dt>domainName</dt>
        <dd>string<em></em></dd>
        <div class="clearfix"></div>               

        <dt>owningOrgName</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>

        <dt>owningOrgId</dt>
        <dd>positive integer<em></em></dd>
        <div class="clearfix"></div>

        <dt>maintainingOrgName</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>

        <dt>maintainingOrgId</dt>
        <dd>positive integer<em></em></dd>
        <div class="clearfix"></div>

          <dt>sourceUrl</dt>
          <dd class="">string</dd>
        <div class="clearfix"></div>
          
        <dt>targetUrl</dt>
        <dd class="">string</dd>
        <div class="clearfix"></div>

        <dt>persistentUrl</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>

        <dt>thumbnailUrl</dt>
        <dd class="">string</dd>
        <div class="clearfix"></div>

        <dt>alternateImages</dt>
        <dd class="">array of objects<em></em></dd>
        <div class="clearfix"></div>

        <dt>alternateText</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>

        <dt>noScriptText</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>

        <dt>featuredText</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>

        <dt>author</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>

        <dt>length</dt>
        <dd class="">string</dd>
        <div class="clearfix"></div>

        <dt>size</dt>
        <dd class="">positive integer<em></em></dd>
        <div class="clearfix"></div>

        <dt>height</dt>
        <dd class="">positive integer<em></em></dd>
        <div class="clearfix"></div>

        <dt>width</dt>
        <dd class="">positive integer<em></em></dd>
        <div class="clearfix"></div>              

        <dt>embedCode</dt>
        <dd class="">string</dd>
        <div class="clearfix"></div> 

        <dt>childCount</dt>
        <dd>positive integer<em></em></dd>
        <div class="clearfix"></div>

        <dt>children</dt>
        <dd>array of objects<em></em></dd>
        <div class="clearfix"></div>

        <dt>parentCount</dt>
        <dd>positive integer<em></em></dd>
        <div class="clearfix"></div>

        <dt>parents</dt>
        <dd>array of objects<em></em></dd>
        <div class="clearfix"></div>

        <dt>rating</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>

        <dt>ratingCount</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>

        <dt>ratingCommentCount</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>

        <dt>status</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>
        
        <dt>datePublished</dt>
        <dd>string</dd>
        <div class="clearfix"></div>   

        <dt>dateModified</dt>
        <dd>string</dd>
        <div class="clearfix"></div>

        <dt>dateContentAuthored</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>

        <dt>dateContentUpdated</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>

        <dt>dateContentPublished</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>

        <dt>dateContentReviewed</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>

        <dt>dateSyndicationCaptured</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>

        <dt>dateSyndicationUpdated</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>

        <dt>dateSyndicationVisible</dt>
        <dd class="">string<em></em></dd>
        <div class="clearfix"></div>

        <dt>extendedAttributes</dt>
        <dd>array of objects<em></em></dd>
        <div class="clearfix"></div>

        <dt>extension</dt>
        <dd class="">object<em></em></dd>
        <div class="clearfix"></div>

  </dl>

</div>

  </div>
  <div class="method-example">
    <code class="method-object"><span class="lang lang-json"><span class="highlight_js bash">{
    <span class="string">"id"</span>: 16,
    <span class="string">"name"</span>: <span class="string">"Tobacco Use - Winnable Battles"</span>,
    <span class="string">"description"</span>: null,
    <span class="string">"mediaType"</span>: <span class="string">"HTML"</span>,
    <span class="string">"language"</span>: {
        <span class="string">"name"</span>: <span class="string">"English"</span>,
        <span class="string">"isoCode"</span>: <span class="string">"eng"</span>
    },
    <span class="string">"tags"</span>: [
        {
          <span class="string">"id"</span>: 10,
          <span class="string">"name"</span>: <span class="string">"Smoking Cessation"</span>,
          <span class="string">"language"</span>: <span class="string">"English"</span>,
          <span class="string">"type"</span>: <span class="string">"Topic"</span>
        }     
    ],  
    <span class="string">"geoTags"</span>: [
        {
          <span class="string">"name"</span>: <span class="string">"Bleckley County"</span>,
          <span class="string">"countryCode"</span>: <span class="string">"US"</span>,
          <span class="string">"geoNameId"</span>: 4183415,
          <span class="string">"parentId"</span>: 4197000,
          <span class="string">"latitude"</span>: 32.43444,
          <span class="string">"longitude"</span>: -83.32784,
          <span class="string">"timezone"</span>: <span class="string">"America/New_York"</span>,
          <span class="string">"admin1Code"</span>: <span class="string">"GA"</span>
        }     
    ],
    <span class="string">"campaigns"</span>: [],
    <span class="string">"source"</span>: {
        <span class="string">"name"</span>: <span class="string">"Centers for Disease Control and Prevention"</span>,
        <span class="string">"acronym"</span>: <span class="string">"CDC"</span>,
        <span class="string">"websiteUrl"</span>: null,
        <span class="string">"largeLogoUrl"</span>: null,
        <span class="string">"smallLogoUrl"</span>: null
    },
    <span class="string">"attribution"</span>: null,    
    <span class="string">"domainName"</span>: <span class="string">"http://www.fda.gov/"</span>,
    <span class="string">"owningOrgName"</span>: null,
    <span class="string">"owningOrgId"</span>: null,
    <span class="string">"maintainingOrgName"</span>: null,
    <span class="string">"maintainingOrgId"</span>: null,
    <span class="string">"sourceUrl"</span>: <span class="string">"http://www......[domain]...../winnablebattles/Tobacco/index.html"</span>,
    <span class="string">"targetUrl"</span>: <span class="string">"http://www......[domain]...../winnablebattles/Tobacco/index.html"</span>,    
    <span class="string">"persistentUrl"</span>: null,    
    <span class="string">"thumbnailUrl"</span>: <span class="string">"<% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media/16/thumbnail"</span>,
    <span class="string">"alternateImages"</span>: [
      {
        <span class="string">"id"</span>: 4,
        <span class="string">"name"</span>: <span class="string">"Facebook"</span>,
        <span class="string">"width"</span>: 155,
        <span class="string">"height"</span>: 84,
        <span class="string">"url"</span>: <span class="string">"<% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/links/4.png"</span>
      },
      {...}
    ],
    <span class="string">"alternateText"</span>: null,
    <span class="string">"noScriptText"</span>: null,
    <span class="string">"featuredText"</span>: null,
    <span class="string">"embedCode"</span>: null,
    <span class="string">"author"</span>: null,
    <span class="string">"length"</span>: <span class="string">""</span>,
    <span class="string">"size"</span>: null,
    <span class="string">"height"</span>: null,
    <span class="string">"width"</span>: null,      
    <span class="string">"childCount"</span>: 0,
    <span class="string">"children"</span>: [],
    <span class="string">"parentCount"</span>: 0,
    <span class="string">"parents"</span>: [],
    <span class="string">"rating"</span>: null,
    <span class="string">"ratingCount"</span>: null,
    <span class="string">"ratingCommentCount"</span>: null,
    <span class="string">"status"</span>: <span class="string">"Published"</span>,
    <span class="string">"datePublished"</span>: <span class="string">"2014-03-18T20:30:00Z"</span>,
    <span class="string">"dateModified"</span>: <span class="string">"2013-04-01 12:00:00"</span>,
    <span class="string">"dateContentAuthored"</span>: null,
    <span class="string">"dateContentUpdated"</span>: null,
    <span class="string">"dateContentPublished"</span>: null,
    <span class="string">"dateContentReviewed"</span>: null,
    <span class="string">"dateSyndicationCaptured"</span>: null,
    <span class="string">"dateSyndicationUpdated"</span>: null,
    <span class="string">"dateSyndicationVisible"</span>: null,
    <span class="string">"extendedAttributes"</span>: [],
    <span class="string">"extension"</span>: {}
}</span>
    </span>
</code>

  </div>
</div>
    
          <a id="search_media" name="search_media" class="section-anchor">&nbsp;</a>
<div class="method-section">
  <div class="method-description">

    <h3>Search</h3>
    <p>
      Retrieve media available in the Content Services system.
    </p>
    
    <div class="method-list">
      <h6>Parameters</h6>
    <p>All parameters are optional. All valid <code>Date</code> parameters are in the <b>ISO 8601</b> format: yyyy-MM-dd<b>T</b>HH:mm:ss<b>Z</b></p>

      <dl class="argument-list">

          <dt>fields</dt>
          <dd>
            string, <em>CSV</em><br />
              first-level{second-level}
            <span>
              You can choose the fields you want returned with the "fields" parameter. 
              This is really useful for reducing the payload of your <b>JSON</b> & <b>JSONP</b> API calls.<br />
              <b>Ex:</b> ...?fields=id,mediatype,tags{name,type}
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>q</dt>
          <dd>
            string <em></em>
            <span>
              Searches topic, name, and description.
              For an <b>exact</b> match use quotation marks (ex: "exact phrase to match").
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>mediaTypes</dt>
          <dd>
            string, <em>CSV</em>
            <span>
            </span>
          </dd>
          <div class="clearfix"></div>
          
          <dt>name</dt>
          <dd>
            string
            <span>
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>nameContains</dt>
          <dd>
            string
            <span>
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>description</dt>
          <dd>
            string
            <span>
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>descriptionContains</dt>
          <dd>
            string
            <span>
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>topic</dt>
          <dd>
            string
            <span>
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>topicIds</dt>
          <dd>
            positive integer, <em>CSV</em>
            <span>
            Enter valid Sub-topic Ids.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>audience</dt>
          <dd>
            string
            <span>
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>languageName</dt>
          <dd>
            string, <em>CSV</em>
            <span>
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>languageIsoCode</dt>
          <dd>
            string, <em>CSV</em>
            <span>
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>sourceName</dt>
          <dd>
            string <em></em>
            <span>
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>sourceNameContains</dt>
          <dd>
            string <em></em>
            <span>
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>sourceAcronym</dt>
          <dd>
            string <em></em>
            <span>
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>sourceAcronymContains</dt>
          <dd>
            string <em></em>
            <span>
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>sourceUrl</dt>
          <dd>
            string <em></em>
            <span>
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>sourceUrlContains</dt>
          <dd>
            string <em></em>
            <span>
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>showChildLevel</dt>
          <dd>
            positive integer <em></em>
            <span>
              Return the descendant items to the specified level in the <code>children</code> attribute of a media item.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>showParentLevel</dt>
          <dd>
            positive integer <em></em>
            <span>
              Return the ancestor items to the specified level in the <code>parents</code> attribute of a media item.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>geoName</dt>
          <dd>
            string <em></em>
            <span>
              Searches the name field of the <code>geoTags</code> object.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>geoNameId</dt>
          <dd>
            positive integer <em></em>
            <span>
              Searches the geoNameId field of the <code>geoTags</code> object.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>geoParentId</dt>
          <dd>
            positive integer <em></em>
            <span>
              Searches the geoParentId field of the <code>geoTags</code> object.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>countryCode</dt>
          <dd>
            string <em></em>
            <span>
              Searches the countryCode field of the <code>geoTags</code> object.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>latitude</dt>
          <dd>
            double <em></em>
            <span>
              Searches the latitude field of the <code>geoTags</code> object.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>longitude</dt>
          <dd>
            double <em></em>
            <span>
              Searches the longitude field of the <code>geoTags</code> object.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>dateContentAuthored</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are equal to the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>contentAuthoredSinceDate</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are equal to greater than the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>
          
          <dt>contentAuthoredBeforeDate</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are less than the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>
          
          <dt>contentAuthoredInRange</dt>
          <dd>
            string, <em>CSV</em>
            <span>
              Return items that are equal to and between the year, month and day ranges.
            </span>
          </dd>
          <div class="clearfix"></div>                   

          <dt>dateContentUpdated</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are equal to the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>contentUpdatedSinceDate</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are equal to greater than the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>
          
          <dt>contentUpdatedBeforeDate</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are less than the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>
          
          <dt>contentUpdatedInRange</dt>
          <dd>
            string, <em>CSV</em>
            <span>
              Return items that are equal to and between the year, month and day ranges.
            </span>
          </dd>
          <div class="clearfix"></div>  

          <dt>dateContentPublished</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are equal to the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>contentPublishedSinceDate</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are equal to greater than the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>
          
          <dt>contentPublishedBeforeDate</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are less than the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>
          
          <dt>contentPublishedInRange</dt>
          <dd>
            string, <em>CSV</em>
            <span>
              Return items that are equal to and between the year, month and day ranges.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>dateContentReviewed</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are equal to the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>contentReviewedSinceDate</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are equal to greater than the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>
          
          <dt>contentReviewedBeforeDate</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are less than the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>
          
          <dt>contentReviewedInRange</dt>
          <dd>
            string, <em>CSV</em>
            <span>
              Return items that are equal to and between the year, month and day ranges.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>dateSyndicationCaptured</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are equal to the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>syndicationCapturedSinceDate</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are equal to greater than the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>
          
          <dt>syndicationCapturedBeforeDate</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are less than the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>
          
          <dt>syndicationCapturedInRange</dt>
          <dd>
            string, <em>CSV</em>
            <span>
              Return items that are equal to and between the year, month and day ranges.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>dateSyndicationUpdated</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are equal to the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>syndicationUpdatedSinceDate</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are equal to greater than the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>
          
          <dt>syndicationUpdatedBeforeDate</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are less than the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>
          
          <dt>syndicationUpdatedInRange</dt>
          <dd>
            string, <em>CSV</em>
            <span>
              Return items that are equal to and between the year, month and day ranges.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>dateSyndicationVisible</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are equal to the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>syndicationVisibleSinceDate</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are equal to greater than the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>
          
          <dt>syndicationVisibleBeforeDate</dt>
          <dd>
            string <em></em>
            <span>
              Return items that are less than the year, month and day.
            </span>
          </dd>
          <div class="clearfix"></div>
          
          <dt>syndicationVisibleInRange</dt>
          <dd>
            string, <em>CSV</em>
            <span>
              Return items that are equal to and between the year, month and day ranges.
            </span>
          </dd>
          <div class="clearfix"></div>

      </dl>
    </div>

    <h5>Returns</h5>
    <p>
      Returns the <a href="#media">media object</a> in the results of the <a href="#response">response</a>.
    </p>
  </div>

  <div class="method-example">

    <div class="part">
      <code class="method-declaration">
      <h6>List of Media:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media</span>      
      <h6>Full-Text search:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?q={STRING}</span>
      <h6>MediaType search:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?mediatypes={MEDIA_TYPE_NAME}</span>
      <h6>Name search:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?name={STRING}</span>
      <h6>Topic search:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?topic={STRING}</span>
      <h6>Topic Id search:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?topicids={CSV}</span>
      <h6>Audience search:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?audience={STRING}</span>
      <h6>Language Name search:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?languagename={STRING}</span>
      <h6>Language IsoCode search:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?languageisocode={STRING}</span>
      <h6>Source Name search:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?sourcename={STRING}</span>
      <h6>Source Acronym search:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?sourceacronym={STRING}</span>
      <h6>Show Child Level search:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media/{ID}?showchildlevel={POSITIVE_INTEGER}</span>
      <h6>Show Parent Level search:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media/{ID}?showparentlevel={POSITIVE_INTEGER}</span>
      <h6>Cache search:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?ttl={POSITIVE_INTEGER}</span>    
      </code>

<!--      <code class="method-request"><span class="prompt"></span><span class="lang lang-json"><span class="highlight_js bash">json <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/media\
   -u sk_test_BQokikJOvBiI2HlWgH4olfQ2: \
   -d amount=400 \
   -d currency=usd \
   -d card=tok_1033Y92eZvKYlo2CWiVsUIQX \
   -d <span class="string">"description=Charge for test@example.com"</span></span></span><span class="lang lang-ruby"><span class="highlight_js ruby"><span class="keyword">require</span> <span class="string">"cdc"</span>
<span class="constant">CDC</span>.api_key = <span class="string">"sk_test_BQokikJOvBiI2HlWgH4olfQ2"</span>


Map&lt;String, Object&gt; chargeParams = <span class="keyword">new</span> HashMap&lt;String, Object&gt;();
chargeParams.put(<span class="string">"amount"</span>, <span class="number">400</span>);
chargeParams.put(<span class="string">"currency"</span>, <span class="string">"usd"</span>);
chargeParams.put(<span class="string">"card"</span>, <span class="string">"tok_1033Y92eZvKYlo2CWiVsUIQX"</span>); <span class="comment">// obtained with CDC.js</span>
chargeParams.put(<span class="string">"description"</span>, <span class="string">"Charge for test@example.com"</span>);
chargeParams.put(<span class="string">"metadata"</span>, initialMetadata);

Charge.create(chargeParams);</span></span></code>-->

<!--      <code class="method-response"><span class="lang lang-json"><span class="highlight_js bash">{
  <span class="string">"id"</span>: <span class="string">"ch_1033qU2eZvKYlo2ColcxeHkQ"</span>,
  <span class="string">"object"</span>: <span class="string">"charge"</span>,
  <span class="string">"created"</span>: 1386175627,
  <span class="string">"livemode"</span>: <span class="literal">false</span>,
  <span class="string">"paid"</span>: <span class="literal">true</span>,
  <span class="string">"amount"</span>: 400,
  <span class="string">"currency"</span>: <span class="string">"usd"</span>,
  <span class="string">"refunded"</span>: <span class="literal">false</span>,
  <span class="string">"card"</span>: {
    <span class="string">"id"</span>: <span class="string">"card_1033qU2eZvKYlo2CktzUrDxB"</span>,
    <span class="string">"object"</span>: <span class="string">"card"</span>,
    <span class="string">"last4"</span>: <span class="string">"4242"</span>,
    <span class="string">"type"</span>: <span class="string">"Visa"</span>,
    <span class="string">"exp_month"</span>: 12,
    <span class="string">"exp_year"</span>: 2014,
    <span class="string">"fingerprint"</span>: <span class="string">"Xt5EWLLDS7FJjR1c"</span>,
    <span class="string">"customer"</span>: null,
    <span class="string">"country"</span>: <span class="string">"US"</span>,
    <span class="string">"name"</span>: null,
    <span class="string">"address_line1"</span>: null,
    <span class="string">"address_line2"</span>: null,
    <span class="string">"address_city"</span>: null,
    <span class="string">"address_state"</span>: null,
    <span class="string">"address_zip"</span>: null,
    <span class="string">"address_country"</span>: null,
    <span class="string">"cvc_check"</span>: <span class="string">"pass"</span>,
    <span class="string">"address_line1_check"</span>: null,
    <span class="string">"address_zip_check"</span>: null
  },
  <span class="string">"captured"</span>: <span class="literal">true</span>,
  <span class="string">"refunds"</span>: [

  ],
  <span class="string">"balance_transaction"</span>: <span class="string">"txn_1032HU2eZvKYlo2CEPtcnUvl"</span>,
  <span class="string">"failure_message"</span>: null,
  <span class="string">"failure_code"</span>: null,
  <span class="string">"amount_refunded"</span>: 0,
  <span class="string">"customer"</span>: null,
  <span class="string">"invoice"</span>: null,
  <span class="string">"description"</span>: <span class="string">"Charge for test@example.com"</span>,
  <span class="string">"dispute"</span>: null,
  <span class="string">"metadata"</span>: {
  }
}</span></span></code>-->

    </div>        

  </div>
</div>

          <a id="retrieve_media" name="retrieve_media" class="section-anchor">&nbsp;</a>
<div class="method-section">
  <div class="method-description">

    <h3>Retrieve media by id</h3>

    <p>
      Retrieves the details of a media that can be syndicated.
    </p>

    <div class="method-list">
      <h6>Parameter</h6>
      <dl>
        <dt>id</dt>
        <dd>
          required
          <span>
            The identifier of the media to be retrieved.
          </span>
        </dd>
        <div class="clearfix"></div>

      </dl>
    </div>

    <h5>Returns</h5>
    <p>
        Returns the <a href="#media">media object</a> in the results of the <a href="#response">response</a> if a valid identifier was provided.
    </p>
  </div>

  <div class="method-example">
    <code class="method-declaration"><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media/{MEDIA_ID}</span></code>    
  </div>
</div>
      
          <a id="sort_media" name="sort_media" class="section-anchor">&nbsp;</a>
<div class="method-section">
  <div class="method-description">

    <h3>Sort</h3>
    <p>
      Order the items returned using the attribute names of the <a href="#media">media object</a>.
    </p>

    <p>
     Ascending is the default sort direction.  The <b>minus</b> sign (—) is used to denote Descending.
    </p>

  <div class="method-list">
    <h6>Parameter</h6>
    <dl class="argument-list"> 
        <dt>sort</dt>
        <dd>
        multiple column (CSV), <em>default is ASC</em>
        <span><em>DESC: </em>use the <b>minus</b> sign (—) before the attribute name.</span>
        </dd>
        <div class="clearfix"></div>

        <dt>order</dt>
        <dd>
        single column, <em>ASC or DESC</em>
        <span>        
        </span>
        </dd>
        <div class="clearfix"></div>

    </dl>
  </div>

    <h5>Returns</h5>
    <p>
      Returns the <a href="#media">media object</a> in the results of the <a href="#response">response</a>.
    </p>

  </div>

  <div class="method-example">
    <code class="method-declaration">
    <h6>ASC:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?sort={ATTRIBUTE_NAME}</span>
    <h6>DESC:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?sort=—{ATTRIBUTE_NAME}</span>
    <h6>ASC:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?sort={ATTRIBUTE_NAME}&order=ASC</span>
    <h6>DESC:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?sort={ATTRIBUTE_NAME}&order=DESC</span>
    </code>        
  </div>
</div>

          <a id="page_media" name="page_media" class="section-anchor">&nbsp;</a>
<div class="method-section">
  <div class="method-description">

    <h3>Pagination</h3>
    <p>
      Divide the returned items into multiple result sets (pages).
    </p>

    <p>The pagination object in the <a href="#response">response</a> (meta) contains the URLs for the current, previous, and next page.</p>
    <div class="method-list">
      <h6>Parameters</h6>
      <dl>

        <dt>max</dt>
        <dd>
        optional, <em>default is 100</em>
        <span>
            A limit on the number of items return per page.
        </span>
        </dd>
        <div class="clearfix"></div>

        <dt>pagenum</dt>
        <dd>
        optional, <em>default is 1</em>
        <span>
            The page to return.
        </span>
        </dd>
        <div class="clearfix"></div>

        <dt>offset</dt>
        <dd>
        optional, <em>default is 0</em>
        <span>
            The number of records to skip in the list of returned items.         
        </span>
        </dd>
        <div class="clearfix"></div>        

      </dl>
    </div>

    <h5>Returns</h5>
    <p>
      Returns the <a href="#media">media object</a> in the results of the <a href="#response">response</a>.
    </p>

  </div>

  <div class="method-example">
    <code class="method-declaration">
    <h6>OPTION 1:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?max={POSITIVE_INTEGER}&pagenum={POSITIVE_INTEGER}</span>    
    <h6>OPTION 2:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?max={POSITIVE_INTEGER}&offset={POSITIVE_INTEGER}</span>
    <h6>OPTION 3:</h6><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media?max={POSITIVE_INTEGER}&offset={POSITIVE_INTEGER}&pagenum={POSITIVE_INTEGER}</span>
    </code>
  </div>
</div>

          <a id="embed_media" name="embed_media" class="section-anchor">&nbsp;</a>
<div class="method-section">
  <div class="method-description">

    <h3>Embed Code</h3>
    <p>
      Retrieve the embedded code used to display and process the syndicated HTML content of a media.
    </p>    
    
    <div class="method-list">
      <h6>Parameters</h6>
    <p>
      All <a href="#syndicate_media">Syndication</a> parameters are valid.
    </p> 
    </div>
  </div>

  <div class="method-example">

    <div class="part">
      <code class="method-declaration"><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media/{MEDIA_ID}/embed</span>    
      </code>
    <code class="method-response"><span class="lang lang-json"><span class="highlight_js bash">"results": <span class="string">"{HTML Embed Code}"</span>
</span></span></code>
    </div>        

  </div>
</div>

          <a id="syndicate_media" name="syndicate_media" class="section-anchor">&nbsp;</a>
<div class="method-section">
  <div class="method-description">

    <h3>Syndication</h3>
    <p>
      Retrieve the syndicated HTML content of a media.
    </p>    
    
    <div class="method-list">
      <h6>Parameters</h6>
      <dl class="argument-list">
          <dt>stripScripts</dt>
          <dd>
            optional, <em>default is true</em>
            <span>
              When this value is set to “true”, JavaScript is stripped from the results.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>stripAnchors</dt>
          <dd>
            optional, <em>default is false</em>
            <span>
              When this value is set to “true”, 
              anchor tags are stripped from the results, 
              converting links into regular text. 
              An Anchor is a HTML or XHTML element used to hyperlink to another document, 
              or another location on the current document.
            </span>
          </dd>
          <div class="clearfix"></div>
          
          <dt>stripImages</dt>
          <dd>
            optional, <em>default is false</em>
            <span>
              When this value is set to “true”, images are stripped from the results.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>stripComments</dt>
          <dd>
            optional, <em>default is true</em>
            <span>
              When this value is set to “true”, comments are stripped from the results.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>stripStyles</dt>
          <dd>
            optional, <em>default is true</em>
            <span>
              When this value is set to “true”, 
              inline styles are stripped from the results. 
              An inline style is a style that is applied 
              to an HTML or XHTML element as an attribute of that element.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>stripBreaks</dt>
          <dd>
            optional, <em>default is false</em>
            <span>
              When this value is set to “true”, <code>br</code> elements are stripped from the results.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>cssClasses</dt>
          <dd>
            optional, <em>default is syndicate</em>
            <span>
              A comma delimited list of class IDs to retrieve from the given URL. 
              A class ID is an attribute of an xhtml node normally used to 
              determine the display characteristics of that element in an HTML browser. 
              Used here as a method of selecting content from a source page. 
              <br /><b>E.g. &lt;div class=&quot;syndicate&quot;&gt;&lt;/div&gt;</b>
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>ids</dt>
          <dd>
            optional <em></em>
            <span>
              A comma delimited list of elements to retrieve from the given URL. 
              An element ID is an attribute of an xhtml node normally used to identify 
              the element for use in programming or for convenience. 
              Used here as a method of selecting content from a source page. 
              <br /><b>E.g. &lt;div id=&quot;content-main&quot;&gt;&lt;/div&gt;</b>
              <br /><b>Note:</b> This parameter overrides class based selections.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>xpath</dt>
          <dd>
            optional<em></em>
            <span>An xpath statement defining what should be retrieved 
            from the URL and returned in the result. XPath is a hierarchical/navigation 
            XML programming tool used for location and selecting XML elements 
            in XML documents. Used here as a method of selecting content from a 
            source XHTML page. Note that this parameter overrides both element 
            and class based selections.
            <br /><b>E.g. xpath=//html:body/descendant::html:*[@id='moreInfo']</b></span>
          </dd>
          <div class="clearfix"></div>

          <dt>oe</dt>
          <dd>
            optional, <em>default is UTF-8</em>
            <span><b>Output encoding.</b> Defines the output format of the syndicated content. 
            <br /><b>E.g. utf-8, iso-8859-1</b></span>
          </dd>
          <div class="clearfix"></div>

          <dt>of</dt>
          <dd>
            optional, <em>default is xhtml</em>
            <span><b>Output format.</b> XHTML (Extensible Hypertext Markup Language) is a spinoff of the 
            hypertext markup language (HTML) used for creating Web pages. 
            It is based on the HTML 4.0 syntax, but has been modified to follow the 
            guidelines of XML. XML (EXtensible Markup Language) is open standard for 
            describing data from the W3C. It is used for defining data elements on a 
            Web page and business-to-business documents. XML uses a similar tag structure as HTML; 
            however, whereas HTML defines how elements are displayed, XML defines what those elements contain.
            <br /><b>E.g. xhtml or xml</b></span>
          </dd>
          <div class="clearfix"></div>
          
          <dt>ns</dt>
          <dd>
            optional, <em>default is cdc</em>
            <span><b>Namespace.</b> Used to decorate (prefix) the tags and ids in the
             results to prevent conflict with existing host page elements. 
             Must contain only upper or lower case letters. 
             An underscore character will be appended by the service.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>nw</dt>
          <dd>
            optional, <em>default is true</em>
            <span><b>New window.</b> When this value is set to “false”, 
            links will not open into a new window. When this value is set to “true”, 
            links are forcibly updated with target="_blank," 
            which causes links to open into a new window.
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>w</dt>
          <dd>
            optional
            <span>
              <b>Width.</b> Set the width of various media types.
            </span>
          </dd>
          <div class="clearfix"></div>   

          <dt>h</dt>
          <dd>
            optional
            <span>
              <b>Height.</b> Set the height of various media types.
            </span>
          </dd>
          <div class="clearfix"></div> 

      </dl>
    </div>
  </div>

  <div class="method-example">

    <div class="part">
      <code class="method-declaration"><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media/{MEDIA_ID}/syndicate</span>    
      </code>
    <code class="method-response"><span class="lang lang-json"><span class="highlight_js bash">"results": {
  <span class="string">"mediaId"</span>: 64,
  <span class="string">"mediaType"</span>: <span class="string">"HTML"</span>,
  <span class="string">"sourceUrl"</span>: <span class="string">"http://www......[domain]...../mmwr/preview/mmwrhtml/mm6120a3.htm"</span>,
  <span class="string">"targetUrl"</span>: <span class="string">"http://www......[domain]...../mmwr/preview/mmwrhtml/mm6120a3.htm"</span>,
  <span class="string">"name"</span>: <span class="string">"Smoking is bad, 1998–2010"</span>,
  <span class="string">"description"</span>: <span class="string">""</span>,
  <span class="string">"content"</span>: <span class="string">"{SYNDICATED HTML}"</span>
}</span></span></code>
    </div>        

  </div>
</div>

          <a id="syndicate_content" name="syndicate_content" class="section-anchor">&nbsp;</a>
<div class="method-section">
  <div class="method-description">

    <h3>Content</h3>
    <p>
      Returns <a href="#syndicate_media">Syndication</a> <code>content</code> as defined by the parameters for a Media Type.
    </p>    
    
    <div class="method-list">
      <h6>Parameters</h6>
      <p>Includes all <a href="#syndicate_media">Syndication</a> Parameters.</p>
      
      <dl class="argument-list">
      </dl>
    </div>
  </div>

  <div class="method-example">

    <div class="part">
      <code class="method-declaration"><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/media/{MEDIA_ID}/content</span>    
      </code>
    </div>        

  </div>
</div>

</div>

        <div class="method">
          <a id="mediatypes" name="mediatypes" class="section-anchor">&nbsp;</a>
<div class="method-section">
  <div class="method-description">

    <h2>Media Types</h2>
    <p>List of the available media types.</p>

<div class="method-list method-attr">
      <h6>Attributes</h6>
  <dl class="argument-list">
          <dt>name</dt>
          <dd class="">string</dd>
        <div class="clearfix"></div>

          <dt>description</dt>
          <dd class="">string</dd>
        <div class="clearfix"></div>

          <dt>displayOrdinal</dt>
          <dd class="">positive integer<span>The order of the object in the database.</span></dd>
        <div class="clearfix"></div>
  </dl>
</div>

  </div>
  <div class="method-example">
      <code class="method-declaration"><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/mediatypes</span>
    </code>
    <code class="method-response"><span class="lang lang-json"><span class="highlight_js bash">"results": [
  {
    <span class="string">"name"</span>: <span class="string">"Video"</span>,
    <span class="string">"description"</span>: <span class="string">"Videos"</span>,
    <span class="string">"displayOrdinal"</span>: 2
  },
  {
    <span class="string">"name"</span>: <span class="string">"HTML"</span>,
    <span class="string">"description"</span>: <span class="string">"HTML Content"</span>,
    <span class="string">"displayOrdinal"</span>: 3
  },
  {...}  
]</span></span></code>

  </div>
</div>
          

        </div>

        <div class="method">
          <a id="topics" name="topics" class="section-anchor">&nbsp;</a>
<div class="method-section">
  <div class="method-description">

    <h2>Topics</h2>
    <p>
      List of the available topics.
    </p>

    <div class="method-list">

          <h6>Parameters</h6>
      <dl>
        <dt>language</dt>
          <dd>
            string, <em>CSV, default is English</em>
            <span>
            </span>
          </dd>
          <div class="clearfix"></div>

        <dt>showChild</dt>
        <dd>
        boolean, <em>default is false</em>
        <span>
            Return sub-topics in the <code>items</code> attribute when set to true.
        </span>
        </dd>
        <div class="clearfix"></div>              
         
          <dt>mediaType</dt>
          <dd>
            string <em></em>
            <span>
                Filter topics using media type.
            </span>
          </dd>
          <div class="clearfix"></div>

      </dl>
    </div>
    
    <div class="method-list">
      <h6>Attributes</h6>
  <dl class="argument-list">
          <dt>id</dt>
          <dd class="">positive integer<em></em></dd>
        <div class="clearfix"></div>

          <dt>name</dt>
          <dd class="">string</dd>
        <div class="clearfix"></div>

          <dt>description</dt>
          <dd class="">string</dd>
        <div class="clearfix"></div>

      <dt>language</dt>
                  <dd class="">string</dd>
                <div class="clearfix"></div>

          <dt>mediaUsageCount</dt>
          <dd class="">positive integer<em></em><span>
                  The number of media items tagged.
                </span>
        </dd>
        <div class="clearfix"></div>

          <dt>displayOrdinal</dt>
          <dd class="">positive integer<span>The order of the object in the database.</span></dd>
        <div class="clearfix"></div>

          <dt>items</dt>
          <dd class="">array of objects<span>Contains the same attributes as the parent object.</span></dd>
        <div class="clearfix"></div>

  </dl>
</div>

  </div>
  
  <div class="method-example">
      <code class="method-declaration"><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/topics</span>
    </code>
    <code class="method-response"><span class="lang lang-json"><span class="highlight_js bash">"results": [
  {
    <span class="string">"id"</span>: 1,
    <span class="string">"name"</span>: <span class="string">"Diseases &amp;amp; Conditions"</span>,
    <span class="string">"description"</span>: <span class="string">"Diseases &amp;amp; Conditions"</span>,
    <span class="string">"language"</span>: <span class="string">"English"</span>,    
    <span class="string">"mediaUsageCount"</span>: 267,
    <span class="string">"displayOrdinal"</span>: 0,
    <span class="string">"items"</span>: [ ]
  },
  {...}      
]</span></span></code>

  </div>

</div></div>

        <div class="method">
                  <a id="audiences" name="audiences" class="section-anchor">&nbsp;</a>
        <div class="method-section">
          <div class="method-description">
    
            <h2>Audiences</h2>
            <p>
              List of the available audiences.
            </p>

        <div class="method-list">

      <h6>Parameters</h6>
      <dl>
          <dt>language</dt>
          <dd>
            string, <em>CSV, default is English</em>
            <span>
            </span>
          </dd>
          <div class="clearfix"></div>

        <div class="clearfix"></div>   

      </dl>
            </div>
          
          <div class="method-list">
              <h6>Attributes</h6>
          <dl class="argument-list">
                  <dt>id</dt>
                  <dd class="">positive integer<em></em></dd>
                <div class="clearfix"></div>

                  <dt>name</dt>
                  <dd class="">string</dd>
                <div class="clearfix"></div>

                  <dt>description</dt>
                  <dd class="">string</dd>
                <div class="clearfix"></div>

               <dt>language</dt>
                  <dd class="">string</dd>
                <div class="clearfix"></div>

                  <dt>mediaUsageCount</dt>
                  <dd class="">positive integer<em></em><span>
                          The number of media items tagged.
                        </span>
                </dd>
                <div class="clearfix"></div>

                  <dt>displayOrdinal</dt>
                  <dd class="">positive integer<span>The order of the object in the database.</span></dd>
                <div class="clearfix"></div>

                  <dt>items</dt>
                  <dd class="">array of objects<span>Contains the same attributes as the parent object.</span></dd>
                <div class="clearfix"></div>

          </dl>
        </div>

          </div>
  
    <div class="method-example">
    <code class="method-declaration"><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/audiences</span>
    </code>
    <code class="method-response"><span class="lang lang-json"><span class="highlight_js bash">"results": [
  {
    <span class="string">"id"</span>: 25,
    <span class="string">"name"</span>: <span class="string">"Parents"</span>,
    <span class="string">"description"</span>: null,
    <span class="string">"language"</span>: <span class="string">"English"</span>,
    <span class="string">"mediaUsageCount"</span>: 1,
    <span class="string">"displayOrdinal"</span>: 0,
    <span class="string">"items"</span>: [ ]
  },
  {...}  
]</span></span></code>

          </div>
        </div></div>

        <div class="method">
          <a id="tags" name="tags" class="section-anchor">&nbsp;</a>
<div class="method-section">
  <div class="method-description">

    <h2>Tags</h2>
    <p>
      List of the available tags.
    </p>

    <div class="method-list">

      <h6>Parameters</h6>
      <dl>
          <dt>language</dt>
          <dd>
            string, <em>CSV</em>
            <span>
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>name</dt>
          <dd>
            string, <em>CSV</em>
            <span>
            Return tag(s) matching the supplied name(s).
            </span>
          </dd>
          <div class="clearfix"></div>

          <dt>nameContains</dt>
          <dd>
            string
            <span>
            Return tags which contain the supplied partial name.
            </span>
          </dd>
        <div class="clearfix"></div>              

          <dt>mediaId</dt>
          <dd>
            positive integer
            <span>
            Return tags associated with the supplied media id.
            </span>
          </dd>
        <div class="clearfix"></div>   

          <dt>typeId</dt>
          <dd>
            positive integer
            <span>
            Return tags belonging to the supplied tag type id.
            </span>
          </dd>
        <div class="clearfix"></div>   

          <dt>typeName</dt>
          <dd>
            string
            <span>
            Return tags belonging to the supplied tag type name.
            </span>
          </dd>
        <div class="clearfix"></div>   

      </dl>
    </div>
    
    <div class="method-list">
      <h6>Attributes</h6>
  <dl class="argument-list">
          <dt>id</dt>
          <dd class="">positive integer<em></em></dd>
        <div class="clearfix"></div>

          <dt>name</dt>
          <dd class="">string</dd>
        <div class="clearfix"></div>

          <dt>language</dt>
          <dd class="">string</dd>
        <div class="clearfix"></div>

          <dt>type</dt>
          <dd class="">string</dd>
        <div class="clearfix"></div>

  </dl>
</div>

  </div>
  
  <div class="method-example">
      <code class="method-declaration"><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/tags</span>
    </code>
    <code class="method-response"><span class="lang lang-json"><span class="highlight_js bash">"results": [
  {
    <span class="string">"id"</span>: 1,
    <span class="string">"name"</span>: <span class="string">"Diseases &amp;amp; Conditions"</span>,
    <span class="string">"language"</span>: <span class="string">"English"</span>,    
    <span class="string">"type"</span>: <span class="string">"Topic"</span>
  },
  {...}      
]</span></span></code>

  </div>

</div>

          <a id="retrieve_tag" name="retrieve_tag" class="section-anchor">&nbsp;</a>
<div class="method-section">
  <div class="method-description">

    <h3>Retrieve tag by id</h3>

    <p>
      Retrieves a tag object using its id.
    </p>

    <div class="method-list">
      <h6>Parameter</h6>
      <dl>
        <dt>id</dt>
        <dd>
          required
          <span>
            The identifier of the tag to be retrieved.
          </span>
        </dd>
        <div class="clearfix"></div>

      </dl>
    </div>

    <h5>Returns</h5>
    <p>
        Returns the <a href="#tags">tag object</a> in the results of the <a href="#response">response</a> if a valid identifier was provided.
    </p>
  </div>

  <div class="method-example">
    <code class="method-declaration"><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/tags/{TAG_ID}</span></code>    
  </div>
</div>

          <a id="retrieve_tag_media" name="retrieve_tag_media" class="section-anchor">&nbsp;</a>
<div class="method-section">
  <div class="method-description">

    <h3>Retrieve media by tag id</h3>

    <p>
      Retrieves media objects using a tag id.
    </p>

    <div class="method-list">
      <h6>Parameter</h6>
      <dl>
        <dt>id</dt>
        <dd>
          required
          <span>
            The identifier of the tag to be retrieved.
          </span>
        </dd>
        <div class="clearfix"></div>

      </dl>
    </div>

    <h5>Returns</h5>
    <p>
        Returns the <a href="#media">media object</a> in the results of the <a href="#response">response</a> if a valid identifier was provided.
    </p>
  </div>

  <div class="method-example">
    <code class="method-declaration"><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/tags/{TAG_ID}/media</span></code>    
  </div>
</div>

          <a id="related_tags" name="related_tags" class="section-anchor">&nbsp;</a>
<div class="method-section">
  <div class="method-description">

    <h3>Retrieve related tags by id.</h3>

    <p>
      Retrieves tag objects related to another tag.
    </p>

    <div class="method-list">
      <h6>Parameter</h6>
      <dl>
        <dt>id</dt>
        <dd>
          required
          <span>
            The identifier of the tag to be retrieved.
          </span>
        </dd>
        <div class="clearfix"></div>

      </dl>
    </div>

    <h5>Returns</h5>
    <p>
        Returns the <a href="#tags">tag object</a> in the results of the <a href="#response">response</a> if a valid identifier was provided.
    </p>
  </div>

  <div class="method-example">
    <code class="method-declaration"><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/tags/{TAG_ID}/related</span></code>    
  </div>
</div>

</div>

        <div class="method">
              <a id="tagtypes" name="tagtypes" class="section-anchor">&nbsp;</a>
    <div class="method-section">
      <div class="method-description">

        <h2>Tag Types</h2>
        <p>
          List of the available tag types.
        </p>
    
        <div class="method-list">
          <h6>Attributes</h6>
      <dl class="argument-list">
              <dt>id</dt>
              <dd class="">positive integer<em></em></dd>
            <div class="clearfix"></div>

              <dt>name</dt>
              <dd class="">string</dd>
            <div class="clearfix"></div>

              <dt>description</dt>
              <dd class="">string</dd>
            <div class="clearfix"></div>
      </dl>
    </div>

  </div>
  
  <div class="method-example">
      <code class="method-declaration"><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/tagtypes</span>
    </code>
    <code class="method-response"><span class="lang lang-json"><span class="highlight_js bash">"results": [
  {
    <span class="string">"id"</span>: 1,
    <span class="string">"name"</span>: <span class="string">"Topic"</span>,
    <span class="string">"description"</span>: <span class="string">"Topic"</span>  
  },
  {
    <span class="string">"id"</span>: 2,
    <span class="string">"name"</span>: <span class="string">"Audience"</span>,
    <span class="string">"description"</span>: <span class="string">"Audience"</span>   
  },
  {...}      
]</span></span></code>

  </div>

</div></div>

        <div class="method">
                  <a id="languages" name="languages" class="section-anchor">&nbsp;</a>
        <div class="method-section">
          <div class="method-description">
    
            <h2>Languages</h2>
            <p>
              List of the available Languages.
            </p>
            
            <div class="method-list">
              <h6>Attributes</h6>
          <dl class="argument-list">
                  <dt>name</dt>
                  <dd class="">string</dd>
                <div class="clearfix"></div>

                  <dt>isoCode</dt>
                  <dd class="">string</dd>
                <div class="clearfix"></div>

<%--                  <dt>displayOrdinal</dt>
                  <dd class="">positive integer<span>The order of the object in the database.</span></dd>
                <div class="clearfix"></div>--%>
          </dl>
        </div>
          </div>
  
    <div class="method-example">
    <code class="method-declaration"><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/languages</span>
    </code>
    <code class="method-response"><span class="lang lang-json"><span class="highlight_js bash">"results": [
  {
    <span class="string">"name"</span>: <span class="string">"English"</span>,
    <span class="string">"isoCode"</span>: <span class="string">"eng"</span>
  },
  {...}  
]</span></span></code>

          </div>
        </div></div>

        <div class="method">
                  <a id="organizations" name="organizations" class="section-anchor">&nbsp;</a>
        <div class="method-section">
          <div class="method-description">
    
            <h2>Organizations</h2>
            <p>
              List of the available Organizations.
            </p>
          
          <div class="method-list">
              <h6>Attributes</h6>
          <dl class="argument-list">
                  <dt>id</dt>
                  <dd class="">positive integer<span></span></dd>
                <div class="clearfix"></div>

                  <dt>name</dt>
                  <dd class="">string</dd>
                <div class="clearfix"></div>
                
                  <dt>website</dt>
                  <dd class="">array of objects</dd>
                <div class="clearfix"></div>

                  <dt>type</dt>
                  <dd class="">string</dd>
                <div class="clearfix"></div>

                  <dt>typeOther</dt>
                  <dd class="">string</dd>
                <div class="clearfix"></div>

                  <dt>description</dt>
                  <dd class="">string</dd>
                <div class="clearfix"></div>

                  <dt>address</dt>
                  <dd class="">string<em></em></dd>
                <div class="clearfix"></div>

                  <dt>addressContinued</dt>
                  <dd class="">string<em></em></dd>
                <div class="clearfix"></div>

                  <dt>city</dt>
                  <dd class="">string<em></em></dd>
                <div class="clearfix"></div>

                  <dt>stateProvince</dt>
                  <dd class="">string<em></em></dd>
                <div class="clearfix"></div>

                  <dt>postalCode</dt>
                  <dd class="">string<em></em></dd>
                <div class="clearfix"></div>

                  <dt>county</dt>
                  <dd class="">string<em></em></dd>
                <div class="clearfix"></div>

                  <dt>country</dt>
                  <dd class="">string<em></em></dd>
                <div class="clearfix"></div>

          </dl>
        </div>
          </div>
  
    <div class="method-example">
    <code class="method-declaration"><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/organizations</span>
    </code>
    <code class="method-response"><span class="lang lang-json"><span class="highlight_js bash">"results": [
  {
    <span class="string">"id"</span>: 356,
    <span class="string">"name"</span>: <span class="string">"CDC"</span>,
    <span class="string">"website"</span>: [
      {
        <span class="string">"url"</span>: <span class="string">"http://m......[domain]....."</span>,
        <span class="string">"isDefault"</span>: <span class="literal">false</span>
      },
      {
        <span class="string">"url"</span>: <span class="string">"http://www......[domain]....."</span>
        <span class="string">"isDefault"</span>: <span class="literal">true</span>
      }
    ],
    <span class="string">"type"</span>: <span class="string">"U.S. Federal Government"</span>,
    <span class="string">"typeOther"</span>: null,
    <span class="string">"description"</span>: <span class="string">"U.S. Federal Government"</span>,
    <span class="string">"address"</span>: <span class="string">"Clifton Rd."</span>,
    <span class="string">"addressContinued"</span>: null,
    <span class="string">"city"</span>: <span class="string">"Atlanta"</span>,
    <span class="string">"stateProvince"</span>: <span class="string">"GA"</span>,
    <span class="string">"postalCode"</span>: <span class="string">"30333"</span>,
    <span class="string">"county"</span>: <span class="string">"Fulton"</span>,
    <span class="string">"country"</span>: <span class="string">"US"</span>
  },
  {...}  
]</span></span></code>

          </div>
        </div></div>

        <div class="method">
                  <a id="organizationtypes" name="organizationtypes" class="section-anchor">&nbsp;</a>
        <div class="method-section">
          <div class="method-description">
    
            <h2>Organization Types</h2>
            <p>
              List of the available Organization Types.
            </p>
          
          <div class="method-list">
              <h6>Attributes</h6>
          <dl class="argument-list">
                  <dt>type</dt>
                  <dd class="">string</dd>
                <div class="clearfix"></div>

                  <dt>description</dt>
                  <dd class="">string</dd>
                <div class="clearfix"></div>

                  <dt>displayOrdinal</dt>
                  <dd class="">positive integer<span>The order of the object in the database.</span></dd>
                <div class="clearfix"></div>
          </dl>
        </div>
          </div>
  
    <div class="method-example">
    <code class="method-declaration"><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/organizationtypes</span>
    </code>
    <code class="method-response"><span class="lang lang-json"><span class="highlight_js bash">"results": [
  {
    <span class="string">"type"</span>: <span class="string">"U.S. Federal Government"</span>,
    <span class="string">"description"</span>: <span class="string">"U.S. Federal Government"</span>,
    <span class="string">"displayOrdinal"</span>: 1
  },
  {...}  
]</span></span></code>

          </div>
        </div></div>

        <div class="method">
                  <a id="sources" name="sources" class="section-anchor">&nbsp;</a>
        <div class="method-section">
          <div class="method-description">
    
            <h2>Sources</h2>
            <p>
              List of the available Sources.
            </p>
          
          <div class="method-list">
              <h6>Attributes</h6>
          <dl class="argument-list">
                <dt>name</dt>
                <dd class="">string<em></em></dd>
            <div class="clearfix"></div>

                <dt>acronym</dt>
                <dd class="">string<em></em></dd>
            <div class="clearfix"></div>

                <dt>websiteUrl</dt>
                <dd class="">string<em></em></dd>
            <div class="clearfix"></div>

                <dt>largeLogoUrl</dt>
                <dd class="">string<em></em></dd>
            <div class="clearfix"></div>

                <dt>smallLogoUrl</dt>
                <dd class="">string<em></em></dd>
            <div class="clearfix"></div>
          </dl>
        </div>
          </div>
  
    <div class="method-example">
    <code class="method-declaration"><span class="lang lang-json"><b>GET</b> <% Response.Write(HostName);%>/api/<% Response.Write(Version);%>/resources/sources</span>
    </code>
    <code class="method-object"><span class="lang lang-json"><span class="highlight_js bash">"results": [
  {
    <span class="string">"name"</span>: <span class="string">"Centers for Disease Control and Prevention"</span>,
    <span class="string">"acronym"</span>: <span class="string">"CDC"</span>,
    <span class="string">"websiteUrl"</span>: <span class="string">"http://www......[domain]....."</span>,
    <span class="string">"largeLogoUrl"</span>: null,
    <span class="string">"smallLogoUrl"</span>: null,
    {...},
    {...}
  }
]</span>
    </span>
</code>
          </div>
        </div></div>

        <div class="method">
          <a id="api_upgrades" name="api_upgrades" class="section-anchor">&nbsp;</a>
<div class="method-section">
  <div class="method-description">

    <h2>API Changes</h2>
    <p>
    Only <b>"backwards-incompatible"</b> changes to the API are reflected.
    </p>
    <p>What changes are consider to be “backwards-compatible”?</p>
    <ul class="backcompat">
      <li>Adding new API resources.</li>
      <li>Adding new optional request parameters to existing API methods.</li>
      <li>Adding new properties to existing API responses.</li>
      <li>Changing the order of properties in existing API responses.</li>
    </ul>
  </div>
  <div class="method-example"></div>
</div>
</div>

            <div class="method">
    <a id="media_upgrades" name="media_upgrades" class="section-anchor">&nbsp;</a>
<div class="method-section">
  <div class="method-description">

    <h3 class="backcompat">Media</h3>
    <p>Details:</p>
    <ul class="backcompat">
        <li>
            Changed "mediaId" <code>string</code> <b>to</b> "id" <code>integer</code>.
        </li>
        <li>
            Changed "title" <b>to</b> "name".
        </li>
        <li>
            Changed "language" <code>string</code> <b>to</b> language <a href="#languages">object</a>.
        </li>
        <li>
            Changed "sourceCode" <code>string</code> <b>to</b> source <a href="#sources">object</a>
        </li>
        <li>
            Changed Date format "yyyy-MM-dd HH:mm:ss" <b>to</b> "yyyy-MM-dd<b>T</b>HH:mm:ss<b>Z</b>"
        </li>
        <li>
            Changed mediaType parameter <b>to</b> mediaTypes.
        </li>
        <li>
            Changed language parameter <b>to</b> languageName.
        </li>
    </ul>

  </div>
  <div class="method-example">
    <code class="method-object"><span class="lang lang-json"><span class="highlight_js bash">{
    <span class="string">"id"</span>: 16,
    <span class="string">"language"</span>: {
        <span class="string">"name"</span>: <span class="string">"English"</span>,
        <span class="string">"isoCode"</span>: <span class="string">"eng"</span>
    },
    <span class="string">"source"</span>: {
        <span class="string">"name"</span>: <span class="string">"Centers for Disease Control and Prevention"</span>,
        <span class="string">"acronym"</span>: <span class="string">"CDC"</span>,
        <span class="string">"websiteUrl"</span>: null,
        <span class="string">"largeLogoUrl"</span>: null,
        <span class="string">"smallLogoUrl"</span>: null
    },
    <span class="string">"datePublished"</span>: <span class="string">"2013-04-01T12:00:00Z"</span>,
    ...
}</span>
    </span>
</code>

  </div>
</div>

</div>        

        <div class="method backcompat">
    <a id="syndication_upgrades" name="syndication_upgrades" class="section-anchor">&nbsp;</a>
        <div class="method-section">
          <div class="method-description">

            <h3>Syndication</h3>
            <p>Details:</p>
            <ul class="backcompat">
                <li>
                    Changed <code>string</code> "mediaId" <b>to</b> <code>integer</code> on the response <a href="#syndicate_media">object</a>.
                </li>
                <li>
                    Changed "title" <b>to</b> "name".
                </li>
                <li>
                    Changed stripAnchor parameter <b>to</b> stripAnchors.
                </li>
                <li>
                    Changed stripImage parameter <b>to</b> stripImages.
                </li>
                <li>
                    Changed stripStyle parameter <b>to</b> stripStyles.
                </li>
                <li>
                    Changed clsids parameter <b>to</b> cssClasses.
                </li>
                <li>
                    Changed elemids parameter <b>to</b> ids.
                </li>
            </ul>
          </div>  
        <div class="method-example"></div>
        </div></div>

        <div class="method backcompat">
    <a id="language_upgrades" name="language_upgrades" class="section-anchor">&nbsp;</a>
        <div class="method-section">
          <div class="method-description">

            <h3>Languages</h3>
            <p>Details:</p>
            <ul class="backcompat">
                <li>
                    Removed the "displayOrdinal" attribute from the response <a href="#languages">object</a>.
                </li>
                <li>
                    Removed the "description" attribute from the response <a href="#languages">object</a>.
                </li>
            </ul>
          </div>  
        <div class="method-example"></div>
        </div></div>
</div></div>

    <div class="footer">
      <div class="suggestion-box">
      </div>
    </div>

    <script src="js/docs.js"></script>
    <script src="js/highlight.js"></script>
    <script src="js/highlighter.js"></script>

  </body>
</html>
