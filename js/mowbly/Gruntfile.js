module.exports = function(grunt) {
	// Project configuration.
	grunt.initConfig({
		pkg: grunt.file.readJSON('package.json'),
		concat: {
			options: {
				// define a string to put between each file in the concatenated output
				separator: '\r\n\r\n',
				banner: '/*	<%= pkg.dest %>-<%= pkg.version %> <%= grunt.template.today("dd-mm-yyyy") %>	*/\r\n' + 
				'/*	Mowbly - Enterprise Mobile Applications Framework.	*/\r\n'
			},
			dist: {
				files:{
					'dist/<%= pkg.dest %>-<%= pkg.version %>.js':[
						'../src/observable.js',
						'../src/utils.js',
						'../src/core.js',
						'../src/preferences.js',
						'../src/page.js',
						'../src/ui.js',
						'../src/log.js',
						'../src/camera.js',
						'../src/contacts.js',
						'../src/device.js',
						'../src/file.js',
						'../src/geolocation.js',
						'../src/http.js',
						'../src/message.js',
						'../src/network.js',
						'../src/database.js',
						'../src/imagegallery.js',
						'../src/external.js',
						'../src/sms.js',
						'../src/version.js'
					]
				},
			}
		},
		uglify: {
			options: {
				// the banner is inserted at the top of the output
				banner: '/*	<%= pkg.dest %>-<%= pkg.version %> <%= grunt.template.today("dd-mm-yyyy") %>	*/\r\n' + 
				'/*	Mowbly - Mobile Applications Framework.	*/\r\n'
			},
			dist: {
				files: {
					'dist/<%= pkg.dest %>-<%= pkg.version %>.min.js':['dist/<%= pkg.dest %>-<%= pkg.version %>.js'],
				}
			}
		}
	});

	grunt.loadNpmTasks('grunt-contrib-concat');
	grunt.loadNpmTasks('grunt-contrib-uglify');

	// Default task(s).
	grunt.registerTask('default', ['concat','uglify']);
};