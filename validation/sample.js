/* esversion: 6 */
/*
* Validate the OpenAPI specification against a target servers
*/
var fs = require('fs');
var swaggerTest = require('../lib/swagger-test');
var assert = require('chai').assert;

describe('Test specification against a target server', function () {

  // Load the specification
  var rootDir = __dirname;
  var buffer  = fs.readFileSync(testDir + '../interop/openapi.json');
  var spec    = JSON.parse(buffer);

  var xamples = swaggerTest.parse(spec, { inferXamples: true });

  // Run through all of the tests.  Will likely need to parse "x-" properties
  // in the specification to properly evaluate request / response pairs.
  for (let xample of xamples) {
    it(xample.title, function() {
      assert.deepEqual(xample, JSON.loads(xample['x-test-response']));
    });
  }
});
