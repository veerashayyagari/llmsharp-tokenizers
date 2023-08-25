using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks
{
    internal class Constants
    {
        internal const string BasicString = "BenchmarkDotNet helps you to transform methods into benchmarks, track their performance, and share reproducible measurement experiments.";
        internal const string OpenAiPluginsDocumentation = @"Consider implementing rate limiting on the API endpoints you expose. ChatGPT will respect 429 response codes and dynamically back off from sending requests to your plugin after receiving a certain number of 429's or 500's in a short period of time.

Timeouts
When making API calls during the plugin experience, timeouts take place if the following thresholds are exceeded:

15 seconds round trip for fetching ai-plugin.json/openapi.yaml
45 seconds round trip for API calls
As we scale the plugin experience to more people, we expect that the timeout thresholds will decrease.

Updating your plugin
Manifest files must be manually updated by going through the ""Develop your own plugin"" flow in the plugin store each time you make a change to the ai-plugin.json file. ChatGPT will automatically fetch the latest OpenAPI spec each time a request is made so changes made will propagate to end users immediately. If your plugin is available in the ChatGPT plugin store and you go through the ""Develop your own plugin"" flow, we will automatically look for changes in the files and remove the plugin if it has changed. You will have to resubmit your plugin to be included in the store again.

Plugin terms
In order to register a plugin, you must agree to the Plugin Terms.

Domain verification and security
To ensure that plugins can only perform actions on resources that they control, OpenAI enforces requirements on the plugin's manifest and API specifications.

Defining the plugin's root domain
The manifest file defines information shown to the user (like logo and contact information) as well as a URL where the plugin's OpenAPI spec is hosted. When the manifest is fetched, the plugin's root domain is established following these rules:

If the domain has www. as a subdomain, then the root domain will strip out www. from the domain that hosts the manifest.
Otherwise, the root domain is the same as the domain that hosts the manifest.
Note on redirects: If there are any redirects in resolving the manifest, only child subdomain redirects are allowed. The only exception is following a redirect from a www subdomain to one without the www.

Examples of what the root domain looks like:

✅ https://example.com/.well-known/ai-plugin.json
Root domain: example.com
✅ https://www.example.com/.well-known/ai-plugin.json
Root domain: example.com
✅ https://www.example.com/.well-known/ai-plugin.json → redirects to https://example.com/.well-known/ai-plugin.json
Root domain: example.com
✅ https://foo.example.com/.well-known/ai-plugin.json → redirects to https://bar.foo.example.com/.well-known/ai-plugin.json
Root domain: bar.foo.example.com
✅ https://foo.example.com/.well-known/ai-plugin.json → redirects to https://bar.foo.example.com/baz/ai-plugin.json
Root domain: bar.foo.example.com
❌ https://foo.example.com/.well-known/ai-plugin.json → redirects to https://example.com/.well-known/ai-plugin.json
Redirect to parent level domain is disallowed
❌ https://foo.example.com/.well-known/ai-plugin.json → redirects to https://bar.example.com/.well-known/ai-plugin.json
Redirect to same level subdomain is disallowed
❌ https://example.com/.well-known/ai-plugin.json -> redirects to https://example2.com/.well-known/ai-plugin.json
Redirect to another domain is disallowed
Manifest validation
Specific fields in the manifest itself must satisfy the following requirements:

api.url - the URL provided to the OpenAPI spec must be hosted at the same level or a subdomain of the root domain.
legal_info - The second-level domain of the URL provided must be the same as the second-level domain of the root domain.
contact_info - The second-level domain of the email address should be the same as the second-level domain of the root domain.
Resolving the API spec
The api.url field in the manifest provides a link to an OpenAPI spec that defines APIs that the plugin can call into. OpenAPI allows specifying multiple server base URLs. The following logic is used to select the server URL:

Iterate through the list of server URLs
Use the first server URL that is either an exact match of the root domain or a subdomain of the root domain
If neither cases above apply, then default to the domain where the API spec is hosted. For example, if the spec is hosted on api.example.com, then api.example.com will be used as the base URL for the routes in the OpenAPI spec.
Note: Please avoid using redirects for hosting the API spec and any API endpoints, as it is not guaranteed that redirects will always be followed.

Use TLS and HTTPS
All traffic with the plugin (e.g., fetching the ai-plugin.json file, the OpenAPI spec, API calls) must use TLS 1.2 or later on port 443 with a valid public certificate.

IP egress ranges
ChatGPT will call your plugin from an IP address in the CIDR block 23.102.140.112/28. You may wish to explicitly allowlist these IP addresses.

Separately, OpenAI's web browsing plugin accesses websites from a different IP address block: 23.98.142.176/28.

FAQ
How is plugin data used?
Plugins connect ChatGPT to external apps. If a user enables a plugin, ChatGPT may send parts of their conversation and their country or state to your plugin.

What happens if a request to my API fails?
If an API request fails, the model might retry the request up to 10 times before letting the user know it cannot get a response from that plugin.

Can I invite people to try my plugin?
Yes, all unverified plugins can be installed by up to 100 other developers who have plugin access. If your plugin is available in the plugin store, it will be accessible to all ChatGPT plus customers.

Can I charge people money for my plugin?
Not at this time.
Consider implementing rate limiting on the API endpoints you expose. ChatGPT will respect 429 response codes and dynamically back off from sending requests to your plugin after receiving a certain number of 429's or 500's in a short period of time.

Timeouts
When making API calls during the plugin experience, timeouts take place if the following thresholds are exceeded:

15 seconds round trip for fetching ai-plugin.json/openapi.yaml
45 seconds round trip for API calls
As we scale the plugin experience to more people, we expect that the timeout thresholds will decrease.

Updating your plugin
Manifest files must be manually updated by going through the ""Develop your own plugin"" flow in the plugin store each time you make a change to the ai-plugin.json file. ChatGPT will automatically fetch the latest OpenAPI spec each time a request is made so changes made will propagate to end users immediately. If your plugin is available in the ChatGPT plugin store and you go through the ""Develop your own plugin"" flow, we will automatically look for changes in the files and remove the plugin if it has changed. You will have to resubmit your plugin to be included in the store again.

Plugin terms
In order to register a plugin, you must agree to the Plugin Terms.

Domain verification and security
To ensure that plugins can only perform actions on resources that they control, OpenAI enforces requirements on the plugin's manifest and API specifications.

Defining the plugin's root domain
The manifest file defines information shown to the user (like logo and contact information) as well as a URL where the plugin's OpenAPI spec is hosted. When the manifest is fetched, the plugin's root domain is established following these rules:

If the domain has www. as a subdomain, then the root domain will strip out www. from the domain that hosts the manifest.
Otherwise, the root domain is the same as the domain that hosts the manifest.
Note on redirects: If there are any redirects in resolving the manifest, only child subdomain redirects are allowed. The only exception is following a redirect from a www subdomain to one without the www.

Examples of what the root domain looks like:

✅ https://example.com/.well-known/ai-plugin.json
Root domain: example.com
✅ https://www.example.com/.well-known/ai-plugin.json
Root domain: example.com
✅ https://www.example.com/.well-known/ai-plugin.json → redirects to https://example.com/.well-known/ai-plugin.json
Root domain: example.com
✅ https://foo.example.com/.well-known/ai-plugin.json → redirects to https://bar.foo.example.com/.well-known/ai-plugin.json
Root domain: bar.foo.example.com
✅ https://foo.example.com/.well-known/ai-plugin.json → redirects to https://bar.foo.example.com/baz/ai-plugin.json
Root domain: bar.foo.example.com
❌ https://foo.example.com/.well-known/ai-plugin.json → redirects to https://example.com/.well-known/ai-plugin.json
Redirect to parent level domain is disallowed
❌ https://foo.example.com/.well-known/ai-plugin.json → redirects to https://bar.example.com/.well-known/ai-plugin.json
Redirect to same level subdomain is disallowed
❌ https://example.com/.well-known/ai-plugin.json -> redirects to https://example2.com/.well-known/ai-plugin.json
Redirect to another domain is disallowed
Manifest validation
Specific fields in the manifest itself must satisfy the following requirements:

api.url - the URL provided to the OpenAPI spec must be hosted at the same level or a subdomain of the root domain.
legal_info - The second-level domain of the URL provided must be the same as the second-level domain of the root domain.
contact_info - The second-level domain of the email address should be the same as the second-level domain of the root domain.
Resolving the API spec
The api.url field in the manifest provides a link to an OpenAPI spec that defines APIs that the plugin can call into. OpenAPI allows specifying multiple server base URLs. The following logic is used to select the server URL:

Iterate through the list of server URLs
Use the first server URL that is either an exact match of the root domain or a subdomain of the root domain
If neither cases above apply, then default to the domain where the API spec is hosted. For example, if the spec is hosted on api.example.com, then api.example.com will be used as the base URL for the routes in the OpenAPI spec.
Note: Please avoid using redirects for hosting the API spec and any API endpoints, as it is not guaranteed that redirects will always be followed.

Use TLS and HTTPS
All traffic with the plugin (e.g., fetching the ai-plugin.json file, the OpenAPI spec, API calls) must use TLS 1.2 or later on port 443 with a valid public certificate.

IP egress ranges
ChatGPT will call your plugin from an IP address in the CIDR block 23.102.140.112/28. You may wish to explicitly allowlist these IP addresses.

Separately, OpenAI's web browsing plugin accesses websites from a different IP address block: 23.98.142.176/28.

FAQ
How is plugin data used?
Plugins connect ChatGPT to external apps. If a user enables a plugin, ChatGPT may send parts of their conversation and their country or state to your plugin.

What happens if a request to my API fails?
If an API request fails, the model might retry the request up to 10 times before letting the user know it cannot get a response from that plugin.

Can I invite people to try my plugin?
Yes, all unverified plugins can be installed by up to 100 other developers who have plugin access. If your plugin is available in the plugin store, it will be accessible to all ChatGPT plus customers.

Can I charge people money for my plugin?
Not at this time.
Consider implementing rate limiting on the API endpoints you expose. ChatGPT will respect 429 response codes and dynamically back off from sending requests to your plugin after receiving a certain number of 429's or 500's in a short period of time.

Timeouts
When making API calls during the plugin experience, timeouts take place if the following thresholds are exceeded:

15 seconds round trip for fetching ai-plugin.json/openapi.yaml
45 seconds round trip for API calls
As we scale the plugin experience to more people, we expect that the timeout thresholds will decrease.

Updating your plugin
Manifest files must be manually updated by going through the ""Develop your own plugin"" flow in the plugin store each time you make a change to the ai-plugin.json file. ChatGPT will automatically fetch the latest OpenAPI spec each time a request is made so changes made will propagate to end users immediately. If your plugin is available in the ChatGPT plugin store and you go through the ""Develop your own plugin"" flow, we will automatically look for changes in the files and remove the plugin if it has changed. You will have to resubmit your plugin to be included in the store again.

Plugin terms
In order to register a plugin, you must agree to the Plugin Terms.

Domain verification and security
To ensure that plugins can only perform actions on resources that they control, OpenAI enforces requirements on the plugin's manifest and API specifications.

Defining the plugin's root domain
The manifest file defines information shown to the user (like logo and contact information) as well as a URL where the plugin's OpenAPI spec is hosted. When the manifest is fetched, the plugin's root domain is established following these rules:

If the domain has www. as a subdomain, then the root domain will strip out www. from the domain that hosts the manifest.
Otherwise, the root domain is the same as the domain that hosts the manifest.
Note on redirects: If there are any redirects in resolving the manifest, only child subdomain redirects are allowed. The only exception is following a redirect from a www subdomain to one without the www.

Examples of what the root domain looks like:

✅ https://example.com/.well-known/ai-plugin.json
Root domain: example.com
✅ https://www.example.com/.well-known/ai-plugin.json
Root domain: example.com
✅ https://www.example.com/.well-known/ai-plugin.json → redirects to https://example.com/.well-known/ai-plugin.json
Root domain: example.com
✅ https://foo.example.com/.well-known/ai-plugin.json → redirects to https://bar.foo.example.com/.well-known/ai-plugin.json
Root domain: bar.foo.example.com
✅ https://foo.example.com/.well-known/ai-plugin.json → redirects to https://bar.foo.example.com/baz/ai-plugin.json
Root domain: bar.foo.example.com
❌ https://foo.example.com/.well-known/ai-plugin.json → redirects to https://example.com/.well-known/ai-plugin.json
Redirect to parent level domain is disallowed
❌ https://foo.example.com/.well-known/ai-plugin.json → redirects to https://bar.example.com/.well-known/ai-plugin.json
Redirect to same level subdomain is disallowed
❌ https://example.com/.well-known/ai-plugin.json -> redirects to https://example2.com/.well-known/ai-plugin.json
Redirect to another domain is disallowed
Manifest validation
Specific fields in the manifest itself must satisfy the following requirements:

api.url - the URL provided to the OpenAPI spec must be hosted at the same level or a subdomain of the root domain.
legal_info - The second-level domain of the URL provided must be the same as the second-level domain of the root domain.
contact_info - The second-level domain of the email address should be the same as the second-level domain of the root domain.
Resolving the API spec
The api.url field in the manifest provides a link to an OpenAPI spec that defines APIs that the plugin can call into. OpenAPI allows specifying multiple server base URLs. The following logic is used to select the server URL:

Iterate through the list of server URLs
Use the first server URL that is either an exact match of the root domain or a subdomain of the root domain
If neither cases above apply, then default to the domain where the API spec is hosted. For example, if the spec is hosted on api.example.com, then api.example.com will be used as the base URL for the routes in the OpenAPI spec.
Note: Please avoid using redirects for hosting the API spec and any API endpoints, as it is not guaranteed that redirects will always be followed.

Use TLS and HTTPS
All traffic with the plugin (e.g., fetching the ai-plugin.json file, the OpenAPI spec, API calls) must use TLS 1.2 or later on port 443 with a valid public certificate.

IP egress ranges
ChatGPT will call your plugin from an IP address in the CIDR block 23.102.140.112/28. You may wish to explicitly allowlist these IP addresses.

Separately, OpenAI's web browsing plugin accesses websites from a different IP address block: 23.98.142.176/28.

FAQ
How is plugin data used?
Plugins connect ChatGPT to external apps. If a user enables a plugin, ChatGPT may send parts of their conversation and their country or state to your plugin.

What happens if a request to my API fails?
If an API request fails, the model might retry the request up to 10 times before letting the user know it cannot get a response from that plugin.

Can I invite people to try my plugin?
Yes, all unverified plugins can be installed by up to 100 other developers who have plugin access. If your plugin is available in the plugin store, it will be accessible to all ChatGPT plus customers.

Can I charge people money for my plugin?
Not at this time.
";
        internal const string AlgebraEquation = @"anxn + an−1xn−1 + … + a2x2 + a1x + a0 = 0,";
        internal const string UnicodeCharacters = "こんにちは世界こんにちは世界こんにちは世界こんにちは世界こんにちは世界こんにちは世界こんにちは世界こんにちは世界こんにちは世界こんにちは世界こんにちは世界こんにちは世界";
    }
}
