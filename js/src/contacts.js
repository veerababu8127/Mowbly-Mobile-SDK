(function(window){
	var mowbly = window.__mowbly__;
	var Bridge = mowbly.Bridge;
	
	/* Address */
	function Address() {}
	Address.FEATURE = "feature";
	Address.ADMIN = "admin";
	Address.SUBADMIN = "subAdmin";
	Address.LOCALITY = "locality";
	Address.THOROUGHFARE = "thoroughfare";
	Address.POSTAL_CODE = "postalCode";
	Address.COUNTRY_CODE = "countryCode";
	Address.COUNTRY_NAME = "country";
	Address.PHONE = "phone";
	Address.URL = "url";
	
	/* Contacts Feature */
	var ContactsFeatureName = "contacts";
	var Contacts = {
		// Contact Constants
		TYPE_FAX : "fax",
		TYPE_HOME : "home",
		TYPE_OTHER : "other",
		TYPE_MOBILE : "mobile",
		TYPE_PAGER : "pager",
		TYPE_WORK : "work",
		
		// Contact properties
		CONTACT_ADDRESSES : "addresses",
		CONTACT_BIRTHDAY : "birthday",
		CONTACT_DEPARTMENT : "department",
		CONTACT_EMAILS : "emails",
		CONTACT_IMPPS : "impps",
		CONTACT_CATEGORIES : "categories",
		CONTACT_JOB_TITLE : "jobTitle",
		CONTACT_NAME : "name",
		CONTACT_PHONES : "phones",
		CONTACT_PHOTOS : "photos",
		CONTACT_URLS : "urls",
		
		// IM Services
		IM_SERVICE_AIM : "aim",
		IM_SERVICE_ICQ : "icq",
		IM_SERVICE_JABBER : "jabber",
		IM_SERVICE_YAHOO : "yahoo",
		
		CATEGORY_BUSINESS : "business",
		CATEGORY_PERSONAL : "personal",

		// A single contact object
		// Can be created with simple strings of name, phone and mail.
		Contact : function(name, phone, email) {
			// Contact properties.
			this.addresses = [];
			this.birthday;
			this.category;
			this.emails = [];
			this.impps = [];
			this.name;
			this.nickname;
			this.note;
			this.organization = {};
			this.phones = [];
			this.photos = [];
			this.urls = [];
			
			this.fieldByType = function(type, aProperty, property) {
				var field;
				var cnt = aProperty.length;
				for(var i=0; i<cnt; i++) {
					var oProperty = aProperty[i];
					if(oProperty["type"] === type) {
						if(property == Contacts.CONTACT_ADDRESSES || property == Contacts.CONTACT_IMPPS) {
							field = oProperty;
						} else {
							field = oProperty.value;
						}
						break;
					}
				}
				return field;
			};

			// methods
			// Getters
			this.getAddress = function(type) {
				type = type || Contacts.TYPE_WORK;
				return this.fieldByType(type, this.addresses, Contacts.CONTACT_ADDRESSES) || {};
			};
			
			this.getHomeAddress = function() {
				return this.getAddress(Contacts.TYPE_HOME);
			};
			
			this.getWorkAddress = function() {
				return this.getAddress(Contacts.TYPE_WORK);
			};
			
			this.getOtherAddress = function() {
				return this.getAddress(Contacts.TYPE_OTHER);
			};
			
			this.getAddresses = function() {
				return this.addresses;
			};
			
			this.getBirthday = function() {
				return new Date(this.birthday).toString("MM/dd/yyyy");
			};

			this.getCategory = function() {
				return this.category;
			};

			this.getEmail = function(type) {
				type = type || Contacts.TYPE_WORK;
				return this.fieldByType(type, this.emails) || "";
			};
			
			this.getHomeEmail = function() {
				return this.getEmail(Contacts.TYPE_HOME);
			};
			
			this.getWorkEmail = function() {
				return this.getEmail(Contacts.TYPE_WORK);
			};
			
			this.getOtherEmail = function() {
				return this.getEmail(Contacts.TYPE_OTHER);
			};

			this.getEmails = function() {
				return this.emails;
			};

			this.getIm = function(type) {
				type = type || IM_SERVICE_JABBER;
				return this.fieldByType(type, this.impps, Contacts.CONTACT_ADDRESSES) || "";
			};
			
			this.getAimIm = function() {
				return this.getIm(Contacts.IM_SERVICE_AIM);
			};
			
			this.getJabberIm = function() {
				return this.getIm(Contacts.IM_SERVICE_JABBER);
			};
			
			this.getIcqIm = function() {
				return this.getIm(Contacts.IM_SERVICE_ICQ);
			};
			
			this.getYahooIm = function() {
				return this.getIm(Contacts.IM_SERVICE_YAHOO);
			};

			this.getIms = function() {
				return this.impps;
			};

			this.getName = function() {
				return this.name;
			};

			this.getNickname = function() {
				return this.nickname;
			};

			this.getNote = function() {
				return this.note;
			};

			this.getOrganization = function() {
				return this.organization;
			};

			this.getPhone = function(type) {
				if(typeof type === "undefined") {
					return this.fieldByType(Contacts.TYPE_WORK, this.phones) ||
							this.fieldByType(Contacts.TYPE_MOBILE, this.phones) ||
							this.fieldByType(Contacts.TYPE_HOME, this.phones) ||
							this.fieldByType(Contacts.TYPE_OTHER, this.phones) ||
							this.fieldByType(Contacts.TYPE_FAX, this.phones) || "";
				} else {
					return this.fieldByType(type, this.phones) || "";
				}
			};
			
			this.getHomePhone = function() {
				return this.getPhone(Contacts.TYPE_HOME);
			};
			
			this.getWorkPhone = function() {
				return this.getPhone(Contacts.TYPE_WORK);
			};
			
			this.getOtherPhone = function() {
				return this.getPhone(Contacts.TYPE_OTHER);
			};
			
			this.getFax = function() {
				return this.getPhone(Contacts.TYPE_FAX);
			};
			
			this.getMobile = function() {
				return this.getPhone(Contacts.TYPE_MOBILE);
			};
			
			this.getPager = function() {
				return this.getPhone(Contacts.TYPE_PAGER);
			};

			this.getPhones = function() {
				return this.phones;
			};

			this.getPhoto = function(type) {
				type = type || Contacts.TYPE_OTHER;
				return this.fieldByType(type, this.photos) || "";
			};

			this.getPhotos = function() {
				return this.photos;
			};

			this.getUrl = function(type) {
				type = type || Contacts.TYPE_WORK;
				return this.fieldByType(type, this.urls) || "";
			};
			
			this.getHomeUrl = function() {
				return this.getUrl(Contacts.TYPE_HOME);
			};
			
			this.getWorkUrl = function() {
				return this.getUrl(Contacts.TYPE_WORK);
			};
			
			this.getOtherUrl = function() {
				return this.getUrl(Contacts.TYPE_OTHER);
			};

			this.getUrls = function() {
				return this.urls;
			};

			// Setters
			// Adds the provided address (ContactField) to contact
			this.addAddress = function(address, type) {
				__m_Utils.checkRequired("address", address, "object");
				__m_Utils.checkRequired("type", type, "string");
				this.addresses.push(new Contacts.ContactAddress(type, address));
				return this;
			};
			
			this.setAddress = function() {
				var noOfArgs = arguments.length;
				if(noOfArgs === 0) {
					throw new TypeError("The parameter address is required.");
				}
				var args = __m_Utils.argumentsAsArray(arguments);
				if(noOfArgs === 1) {
					args.push(Contacts.TYPE_WORK);
				}
				return this.addAddress.apply(this, args);
			};
			
			// Adds the provided email (ContactField) to contact
			this.addEmail = function(value, type) {
				__m_Utils.checkRequired("type", type, "string");
				__m_Utils.checkRequired("email", value, "string");
				var email = new Contacts.ContactField(type, value);
				this.emails.push(email);
				return this;
			};
			
			this.setEmail = function() {
				var noOfArgs = arguments.length;
				if(noOfArgs === 0) {
					throw new TypeError("The parameter email is required.");
				}
				var args = __m_Utils.argumentsAsArray(arguments);
				if(noOfArgs === 1) {
					args.push(Contacts.TYPE_WORK);
				}
				return this.addEmail.apply(this, args);
			};

			// Adds the provided IM detail (ContactField) to contact
			this.addIm = function(value, type) {
				__m_Utils.checkRequired("type", type, "string");
				__m_Utils.checkRequired("IM", value, "string");
				var im = new Contacts.ContactField(type, value);
				this.impps.push(im);
				return this;
			};
			
			this.setIm = function() {
				var noOfArgs = arguments.length;
				if(noOfArgs === 0) {
					throw new TypeError("The parameter im is required.");
				}
				var args = __m_Utils.argumentsAsArray(arguments);
				if(noOfArgs === 1) {
					args.push(Contacts.IM_SERVICE_JABBER);
				}
				return this.addIm.apply(this, args);
			};

			// Adds the provided phone (ContactField) to contact
			this.addPhone = function(value, type) {
				__m_Utils.checkRequired("type", type, "string");
				__m_Utils.checkRequired("phone", value, ["string", "number"]);
				value = "" + value;
				var phone = new Contacts.ContactField(type, value);
				this.phones.push(phone);
				return this;
			};
			
			this.setPhone = function() {
				var noOfArgs = arguments.length;
				if(noOfArgs === 0) {
					throw new TypeError("The parameter phone is required.");
				}
				var args = __m_Utils.argumentsAsArray(arguments);
				if(noOfArgs === 1) {
					args.push(Contacts.TYPE_WORK);
				}
				return this.addPhone.apply(this, args);
			};

			// Adds the provided photo (ContactField) to contact
			this.addPhoto = function(value, type) {
				__m_Utils.checkRequired("photo", value, ["string", mowbly.File]);
				if(typeof value === "string") {
					value = new mowbly.File(value);
				}
				var photo = new Contacts.ContactField(type, value);
				this.photos.push(photo);
				return this;
			};

			this.setPhoto = function() {
				var args = __m_Utils.argumentsAsArray(arguments);
				args.push(Contacts.TYPE_OTHER);
				return this.addPhoto.apply(this, args);
			};

			// Adds the provided url (ContactField) to contact
			this.addUrl = function(value, type) {
				__m_Utils.checkRequired("type", type, "string");
				__m_Utils.checkRequired("url", value, "string");
				var url = new Contacts.ContactField(type, value);
				this.urls.push(url);
				return this;
			};
			
			this.setUrl = function() {
				var noOfArgs = arguments.length;
				if(noOfArgs === 0) {
					throw new TypeError("The parameter url is required.");
				}
				var args = __m_Utils.argumentsAsArray(arguments);
				if(noOfArgs === 1) {
					args.push(Contacts.TYPE_WORK);
				}
				return this.addUrl.apply(this, args);
			};
			
			this.setBirthday = function(birthday) {
				__m_Utils.checkRequired("birthday", birthday);
				if(typeof birthday == "number") {
					this.birthday = birthday;
				} else {
					// convert the day to millisecs from Jan 1, 1970
					var bdatems = Date.parse(birthday);
					if(! isNaN(bdatems)) {
						this.birthday = bdatems;
					} else {
						throw new TypeError("Birthday is not valid.");
					}
				}
				return this;
			}
			
			// Sets the provided Category (String) to contact
			this.setCategory = function(category) {
				__m_Utils.checkRequired("category", category, "string");
				this.category = category;
				return this;
			};

			// Sets the provided ContactName to the contact
			this.setName = function(firstName, lastName, middleName, prefix, suffix) {
				__m_Utils.checkRequired("name", firstName, "string");
				var name = new Contacts.ContactName(firstName, lastName, middleName, prefix, suffix);
				this.name = name;
				return this;
			};
			
			this.setNickname = function(nickname) {
				if(arguments.length === 0) {
					throw new TypeError("Nickname cannot be empty.");
				}
				this.nickname = nickname;
				return this;
			};
			
			this.setNote = function(note) {
				if(arguments.length === 0) {
					throw new TypeError("Note cannot be empty.");
				}
				this.note = note;
				return this;
			};
			
			// sets the provided Organization (ContactOrganization) to contact
			this.setOrganization = function(name, department, jobTitle) {
				if(arguments.length === 0) {
					throw new TypeError("Organization cannot be empty.");
				}
				var org = new Contacts.ContactOrganization(name, department, jobTitle);
				this.organization = org;
				return this;
			};
			
			// Delete properties
			this.removeFieldByType = function(type, arr) {
				__m_Utils.checkRequired("type", type, "string");
				
				var value;
				var cnt = arr.length, index = -1;
				for(var i=0; i<cnt; i++) {
					if(arr[i].type == type) {
						index = i;
						break;
					}
				}
				
				if(index > -1) {
					arr.splice(index, 1);
				}
			}
			
			this.removeAddress = function(type) {
				this.removeFieldByType(type, this.addresses);
				return this;
			};
			
			this.removeHomeAddress = function() {
				this.removeAddress(Contacts.TYPE_HOME);
				return this;
			};
			
			this.removeWorkAddress = function() {
				this.removeAddress(Contacts.TYPE_WORK);
				return this;
			};
			
			this.removeOtherAddress = function() {
				this.removeAddress(Contacts.TYPE_OTHER);
				return this;
			};
			
			this.removeAddresses = function() {
				delete this.addresses;
				return this;
			};
			
			this.removeBirthday = function() {
				delete this.birthday;
				return this;
			};

			this.removeCategory = function() {
				delete this.category;
				return this;
			};

			this.removeEmail = function(type) {
				this.removeFieldByType(type, this.emails);
				return this;
			};
			
			this.removeHomeEmail = function() {
				this.removeEmail(Contacts.TYPE_HOME);
				return this;
			};
			
			this.removeWorkEmail = function() {
				this.removeEmail(Contacts.TYPE_WORK);
				return this;
			};
			
			this.removeOtherEmail = function() {
				this.removeEmail(Contacts.TYPE_OTHER);
				return this;
			};

			this.removeEmails = function() {
				delete this.emails;
				return this;
			};

			this.removeIm = function(type) {
				this.removeFieldByType(type, this.m_impps);
				return this;
			};
			
			this.removeAimIm = function() {
				this.removeIm(Contacts.IM_SERVICE_AIM);
				return this;
			};
			
			this.removeJabberIm = function() {
				this.removeIm(Contacts.IM_SERVICE_JABBER);
				return this;
			};
			
			this.removeIcqIm = function() {
				this.removeIm(Contacts.IM_SERVICE_ICQ);
				return this;
			};
			
			this.removeYahooIm = function() {
				this.removeIm(Contacts.IM_SERVICE_YAHOO);
				return this;
			};

			this.removeIms = function() {
				delete this.impps;
				return this;
			};

			this.removeName = function() {
				delete this.name;
				return this;
			};

			this.removeNickname = function() {
				delete this.nickname;
				return this;
			};

			this.removeNote = function() {
				delete this.note;
				return this;
			};

			this.removeOrganization = function() {
				delete this.organization;
				return this;
			};

			this.removePhone = function(type) {
				this.removeFieldByType(type, this.phones);
			};

			this.removePhones = function() {
				delete this.phones;
				return this;
			};

			this.removePhoto = function(type) {
				this.removeFieldByType(type, this.photos);
				return this;
			};

			this.removePhotos = function() {
				delete this.photos;
				return this;
			};

			this.removeUrl = function(type) {
				this.removeFieldByType(type, this.urls);
				return this;
			};
			
			this.removeHomeUrl = function() {
				this.removeUrl(Contacts.TYPE_HOME);
				return this;
			};
			
			this.removeWorkUrl = function() {
				this.removeUrl(Contacts.TYPE_WORK);
				return this;
			};
			
			this.removeOtherUrl = function() {
				this.removeUrl(Contacts.TYPE_OTHER);
				return this;
			};

			this.removeUrls = function() {
				// TODO Fix!
				return this.urls;
				return this;
			};
			
			this.remove = function(fp_callback) {
				if(Bridge.getType() !== Bridge.WINDOWSPHONE) {
					if(typeof this.id === "undefined") {
						throw new TypeError("Invalid contact.");
					}
				}
				Bridge.invoke(ContactsFeatureName, "deleteContact", [this.id], fp_callback);
			};
		
			this.save = function(fp_callback) {
				var that = this;
				var fp_onContactSaved = function(response) {
					if(response.code) {
						// Update the ID of the contact
						that.id = response.result;
					}
					response.result = that;
					fp_callback(response);
				}
				Bridge.invoke(ContactsFeatureName, "saveContact", [this], fp_onContactSaved);
			};
			
			this.view = function() {
				var param;
				if(Bridge.getType() !== Bridge.WINDOWSPHONE) {
					param = this.id;
					if(typeof this.id === "undefined") {
						throw new TypeError("Could not view contact. Unique id is missing. Save the contact to get unique id.");
					}
				} else {
					param = this;
				}
				Bridge.invoke(ContactsFeatureName, "viewContact", [param]);
			};

			// Set the properties specified
			if(name) {
				if(typeof name === "string") {
					this.setName(name);
				} else if(	typeof name === "object" && 
							name instanceof Contacts.ContactName &&
							name.firstName) {
					this.name = name;
				}
			}

			if(phone) {
				if(typeof phone === "string" || typeof phone === "number") {
					this.setPhone(phone);
				} else if(__m_Utils.isArray(phone)) {
					this.phones = phone;
				}
			}

			if(email) {
				if(typeof(email) === "string") {
					this.setEmail(email);
				} else if(__m_Utils.isArray(email)) {
					this.emails = email;
				}
			}
		},

		ContactName : function(firstName, lastName, middleName, prefix, suffix) {
			this.firstName = firstName;
			this.lastName = lastName;
			this.middleName = middleName;
			this.prefix = prefix;
			this.suffix = suffix;
			
			this.search = function(text){
				return __m_Utils.search(text,this);
			}
		},

		ContactAddress : function(type, address) {
			this.type = type;
			this.street = address.street;
			this.city = address.city;
			this.region = address.region;
			this.postalCode = address.postalCode;
			this.country = address.country;
			this.additionalInfo = address.additionalInfo;
			
			this.search = function(text){
				return __m_Utils.search(text,this);
			}
		},

		ContactField : function(type, value) {
			this.type = type;
			this.value = value;
			
			this.search = function(text){
				return __m_Utils.search(text,this);
			}
		},

		ContactOrganization : function(name, department, jobTitle) {
			this.name = name;
			this.department = department;
			this.jobTitle = jobTitle;
			
			this.search = function(text){
				return __m_Utils.search(text,this);
			}
		},
		
		ContactOptions : function(properties, limit) {
			if(typeof limit == "undefined") {
				limit = 0;	// Get all records
			}
			this.limit = limit;
			// properties - ["firstName", "phoneNumber"];
			this.properties = properties;
		},

		ContactError : {
			UNKNOWN_ERR : 0,
			INVALID_ARGUMENT_ERROR : 1,
			CONTACT_NOT_FOUND_ERROR : 30,
			CONTACT_INVALID_ERROR : 31
		},

		// Methods
		create : function(properties, phone, email) {
			if(typeof properties === "string" || typeof properties === "undefined") {
				var name = properties;
				return new this.Contact(name, phone, email);
			} else if(typeof properties === "object") {
				var createFieldByType = function(field, property) {
					var retField = {};
					if(property == Contacts.CONTACT_ADDRESSES || property == Contacts.CONTACT_IMPPS) {
						for(var i in field) {
							retField[field[i].type] = field[i];
						}
					} else {
						for(var i in field) {
							retField[field[i].type] = field[i].value;
						}
					}
					return retField;
				};
			
			
				// Create contact from contact properties object				
				var contact = this.create();
				// set contact properties
				// Id
				contact.id = properties.id;
				
				// Address - Home, Work, Other
				if(properties.addresses && properties.addresses.length > 0) {
					var addresses = createFieldByType(properties.addresses, Contacts.CONTACT_ADDRESSES);
					if(addresses) {
						var hAddr = addresses[this.TYPE_HOME];
						if(hAddr){
							contact.addAddress({"type" : hAddr.type,
												"street" : hAddr.street,
												"city" : hAddr.city,
												"region" : hAddr.region,
												"postalCode" : hAddr.postalCode,
												"country" : hAddr.country,
												"additionalInfo" : hAddr.additionalInfo}, hAddr.type);
							}
						var wAddr = addresses[this.TYPE_WORK];
						if(wAddr){
							contact.addAddress({"type" : wAddr.type,
												"street" : wAddr.street,
												"city" : wAddr.city,
												"region" : wAddr.region,
												"postalCode" : wAddr.postalCode,
												"country" : wAddr.country,
												"additionalInfo" : wAddr.additionalInfo}, wAddr.type);
						}
						var oAddr = addresses[this.TYPE_OTHER];
						if(oAddr){
							contact.addAddress({"type" : oAddr.type,
												"street" : oAddr.street,
												"city" : oAddr.city,
												"region" : oAddr.region,
												"postalCode" : oAddr.postalCode,
												"country" : oAddr.country,
												"additionalInfo" : oAddr.additionalInfo}, oAddr.type);
						}
					}
				}

				// Birthday
				if(properties.birthday) {
					contact.setBirthday(properties.birthday);
				}

				// Email - Home, Work, Other
				if(properties.emails) {
					var emails = createFieldByType(properties.emails);
					if(emails) {
						if(emails[this.TYPE_HOME]) contact.setEmail(emails[this.TYPE_HOME], this.TYPE_HOME);
						if(emails[this.TYPE_WORK]) contact.setEmail(emails[this.TYPE_WORK], this.TYPE_WORK);
						if(emails[this.TYPE_OTHER]) contact.setEmail(emails[this.TYPE_OTHER], this.TYPE_OTHER);
					}
				}

				// Impps - Aim, Icq, Jabber, Yahoo
				if(properties.impps && properties.impps.length > 0) {
					var im = createFieldByType(properties.impps, Contacts.CONTACT_IMPPS);
					if(im) {
						if(im[this.IM_SERVICE_AIM]) contact.setIm(im[this.IM_SERVICE_AIM].value, this.IM_SERVICE_AIM);
						if(im[this.IM_SERVICE_ICQ]) contact.setIm(im[this.IM_SERVICE_ICQ].value, this.IM_SERVICE_ICQ);
						if(im[this.IM_SERVICE_JABBER]) contact.setIm(im[this.IM_SERVICE_JABBER].value, this.IM_SERVICE_JABBER);
						if(im[this.IM_SERVICE_YAHOO]) contact.setIm(im[this.IM_SERVICE_YAHOO].value, this.IM_SERVICE_YAHOO);
					}
				}

				// Name
				var name = properties.name;
				if(name) {
					contact.setName(name.firstName,
									name.lastName,
									name.middleName,
									name.prefix,
									name.suffix);
				}

				// Nickname
				if(properties.nickname) {
					contact.setNickname(properties.nickname);
				}

				// Note
				if(properties.note) {
					contact.setNote(properties.note);
				}

				// Category
				if(properties.category) {
					contact.setCategory(properties.category);					
				}

				// Organizations
				var org = properties.organization;
				if(org) {
					contact.setOrganization(org[this.CONTACT_NAME],
											org[this.CONTACT_DEPARTMENT],
											org[this.CONTACT_JOB_TITLE]);
				}

				// Phones
				if(properties.phones) {
					var phones = createFieldByType(properties.phones);
					if(phones) {
						if(phones[this.TYPE_HOME]) contact.setPhone(phones[this.TYPE_HOME], this.TYPE_HOME);
						if(phones[this.TYPE_WORK]) contact.setPhone(phones[this.TYPE_WORK], this.TYPE_WORK);
						if(phones[this.TYPE_MOBILE]) contact.setPhone(phones[this.TYPE_MOBILE], this.TYPE_MOBILE);
						if(phones[this.TYPE_PAGER]) contact.setPhone(phones[this.TYPE_PAGER], this.TYPE_PAGER);
						if(phones[this.TYPE_FAX]) contact.setPhone(phones[this.TYPE_FAX], this.TYPE_FAX);
						if(phones[this.TYPE_OTHER]) contact.setPhone(phones[this.TYPE_OTHER], this.TYPE_OTHER);
					}
				}

				// Photos
				var photos = properties.photos;
				if(photos) {
					var count = photos.length;
					for(var i=0; i<count; i++) {
						var data = photos[i].value;
						if(data && data.value != "") {
							contact.addPhoto(photos[i].type || this.TYPE_WORK, data);
						}
					}
				}

				// Urls - Home, Work, Other
				if(properties.urls) {
					var urls = createFieldByType(properties.urls);
					if(urls) {
						if(urls[this.TYPE_HOME]) contact.setUrl(urls[this.TYPE_HOME], this.TYPE_HOME);
						if(urls[this.TYPE_WORK]) contact.setUrl(urls[this.TYPE_WORK], this.TYPE_WORK);
						if(urls[this.TYPE_OTHER]) contact.setUrl(urls[this.TYPE_OTHER], this.TYPE_OTHER);
					}
				}

				return contact;
			}
		},

		call : function(phoneNumber, bForce) {
			__m_Utils.checkRequired("phoneNumber", phoneNumber, ["string", "number", mowbly.Contacts.Contact]);
			if(phoneNumber instanceof mowbly.Contacts.Contact) {
				phoneNumber = phoneNumber.getPhone();
				__m_Utils.checkRequired("phoneNumber", phoneNumber, ["string", "number"]);
			}
			// Forcing is not there in spec - http://tools.ietf.org/html/rfc3966 Search for word Consent
			if(!bForce){
				mowbly.Ui.confirm({
						"message": "Call " + phoneNumber, 
						"buttons": [{"label": "Call"},
								{"label": "Cancel"}]
					}, function(index){
						if(index == 0) {
							Bridge.invoke(ContactsFeatureName, "callContact", ["" + phoneNumber]);
						}
					}
				);
			}else{
				Bridge.invoke(ContactsFeatureName, "callContact", ["" + phoneNumber]);
			}
		},
		
		find : function(filter) {
			var options = new Contacts.ContactOptions([], 0), fp_callback, noOfArgs = arguments.length;
			__m_Utils.checkRequired("filter", filter, "string");
		
			if(noOfArgs > 1) {
				if(noOfArgs == 2) {
					arg1 = arguments[1];
					__m_Utils.checkType("second argument", arg1, ["object", "function"]);
					if(typeof arg1 === "function") {
						fp_callback = arg1;
					} else {
						options = arg1;
					}
				} else {
					options = arguments[1];
					__m_Utils.checkType("options", options, "object");
					fp_callback = arguments[2];
					__m_Utils.checkType("fp_callback", fp_callback, "function");
				}
			}

			var that = this;
			var fp_onContactFound = function(response) {
				if(response.code) {
					var contacts = [];
					// Create contact objects
					for(var i in response.result) {
						var contactInfo = response.result[i];
						contacts.push(that.create(contactInfo));
					}
					// Replace the result with contacts
					response.result = contacts;
				}
				if(typeof fp_callback === "function") {
					fp_callback(response);
				}
			}
			
			Bridge.invoke(ContactsFeatureName, "findContact", [filter, options], fp_onContactFound);
		},

		pick : function() {
			var options = {}, fp_callback, noOfArgs = arguments.length;
			if(noOfArgs == 1) {
				var arg0 = arguments[0];
				__m_Utils.checkType("first argument", arg0, ["object", "function"]);
				
				if(typeof arg0 === "object") {
					options = arg0;
				} else {
					fp_callback = arg0;
				}
			} else if(noOfArgs > 1) {
				options = arguments[0];
				fp_callback = arguments[1];
				__m_Utils.checkType("options", options, "object");
				__m_Utils.checkType("fp_callback", fp_callback, "function");
			}
		
			var defaultOptions = {
				"filter": [],
				"multiple": false,
				"bChooseProperty": false,
				"bPerformDefaultAction": false
			}

			var opts = __m_Utils.extendOptions({}, defaultOptions, options);

			var that = this;
			var fp_onContactPicked = function(response) {
				if(response.code) {
					var contacts = [];
					if(__m_Utils.isArray(response.result)) {
						// Create contact objects
						var numContacts = response.result.length;
						for(var i = 0; i < numContacts; i++) {
							var contactInfo = response.result[i];
							contacts.push(that.create(contactInfo));
						}
					} else {
						// Single object picked
						contacts.push(that.create(response.result));
					}
					// Replace the result with contacts
					response.result = contacts;
				}
				if(typeof fp_callback === "function") {
					fp_callback(response);
				}
			}

			Bridge.invoke(ContactsFeatureName, "pickContact", [opts.filter, opts.multiple, opts.bChooseProperty, opts.bPerformDefaultAction], fp_onContactPicked);
		}
	};
	
	window.mowbly.contact = function(name, phone, email) {
		var contact = __mowbly__.Contacts.create(name, phone, email);
		return contact;
	};
	
	// Add event library to features.
	__m_Utils.inherit(Contacts, Observable);
	
	mowbly.addFeature(null, "Address", Address);
	mowbly.addFeature(ContactsFeatureName, "Contacts", Contacts);
	
	// Contacts
	mowbly.exportHelperMethod("callContact", "call", "Contacts");
	mowbly.exportHelperMethod("findContact", "find", "Contacts");
	mowbly.exportHelperMethod("pickContact", "pick", "Contacts");
})(window);