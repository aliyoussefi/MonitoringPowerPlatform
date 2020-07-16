<script type="text/javascript">
  var appInsights=window.appInsights||function(config){
    function i(config){t[config]=function(){var i=arguments;t.queue.push(function(){t[config].apply(t,i)})}}var t={config:config},u=document,e=window,o="script",s="AuthenticatedUserContext",h="start",c="stop",l="Track",a=l+"Event",v=l+"Page",y=u.createElement(o),r,f;y.src=config.url||"https://az416426.vo.msecnd.net/scripts/a/ai.0.js";u.getElementsByTagName(o)[0].parentNode.appendChild(y);try{t.cookie=u.cookie}catch(p){}for(t.queue=[],t.version="1.0",r=["Event","Exception","Metric","PageView","Trace","Dependency"];r.length;)i("track"+r.pop());return i("set"+s),i("clear"+s),i(h+a),i(c+a),i(h+v),i(c+v),i("flush"),config.disableExceptionTracking||(r="onerror",i("_"+r),f=e[r],e[r]=function(config,i,u,e,o){var s=f&&f(config,i,u,e,o);return s!==!0&&t["_"+r](config,i,u,e,o),s}),t
    }({
        instrumentationKey:"7b18b7f7-3daf-4951-abba-8372cf9b21a9"
    });
       
    window.appInsights=appInsights;
 
 

{% if user %}
var crmContactId = "{{ user.id }}";
var crmContactName = "{{ user.fullname }}";
var crmContactAccountName = "NoAccount";
{% if user.parentcustomerid %}
crmContactAccountName= "{{ user.parentcustomerid.name }}";
{% endif %}
var crmContactUserString = "{{ user.fullname }} from {{ user.parentcustomerid.name }} (ContactID:{{ user.id }})";

var crmContactRoles = "{{ user.roles }}";
var crmBadges = "{{ user.basic_badges_url }}";

var crmEvents = "{{ events }}";
var crmPage = "{{ page }}";
var validatedId = crmContactUserString.replace(/[,;=| ]+/g, "_"); 
appInsights.setAuthenticatedUserContext(crmContactId);
appInsights.trackEvent({name: 'Test for Monitoring article'});
{% endif %}
 
    appInsights.trackPageView();
 appInsights.trackEvent({name: window.Microsoft.Dynamic365.Portal.type});
</script>
