(function(window){
	var mowbly = window.__mowbly__;
	var Bridge = mowbly.Bridge;	
	
	var DatabaseFeatureName = "database";
	var DatabaseShell = (function(){
		var queryQueue = {};	/* Private Query queue */
		var commandQueue = [];	

		/* Custom dberror object */
		function DBError(code,msg){
			this.code=code;
			this.message = msg + " Reason : " +  this.toString();
		}
		
		function extend(child,parent){
			function F(){};
			F.prototype = parent;
			var f = new F();
			child.prototype=f;
			child.prototype.constructor = child;
			child.parent = parent;
			return child;
		}

		// extend Error object
		extend(DBError,Error);		
	
		/* Error constansts */
		DBError.SQL_ERR = 100;
		DBError.UNKNOWN_ERR = 1000;
		DBError.DATABASE_ERR = 2000;
		DBError.VERSION_ERR = 3000;
		DBError.TOO_LARGE_ERR = 4000;
		DBError.QUOTA_ERR = 5000;
		DBError.SYNTAX_ERR = 6000;
		DBError.CONSTRAINT_ERR = 7000;
		DBError.TIMEOUT_ERR = 8000;
	
		DBError.prototype.toString= function(){
			switch(this.code){
				case DBError.UNKNOWN_ERR :
					return "Unknown error";
				case DBError.DATABASE_ERR :
					return "Database error";
				case DBError.VERSION_ERR :
					return "Version error";
				case DBError.TOO_LARGE_ERR :
					return "Too large error";
				case DBError.QUOTA_ERR :
					return "Quota error";
				case DBError.SYNTAX_ERR :
					return "Syntax error";
				case DBError.CONSTRAINT_ERR :
					return "Constraint error";
				case DBError.TIMEOUT_ERR :
					return "Timeout error";
				default: return "SQL error";
			}
		};
	
		//Returns custom sql error  
		function getSqlerror(error){
			var msg = "";
			if(error && error.message) {
				msg = error.message;
			}
		
			var dbError = new DBError(DBError.UNKNOWN_ERR,msg);
			return dbError;		
		}
	
	
		var DBResultset = function(){
			this.rowsAffected = 0;
			this.insertId = 0;
			this.rows = null;
		};
		
		/* DBSQLCommand object */
		function DBSQLCommand(command,args,name,successCallback,errorCallback){	
			this.command = command; /* DBCommand - execution entity of this native api call */
			this.args=args || [];
			this.name = name;
			this.successCallback = successCallback; /* success callback function */
			this.errorCallback = errorCallback; /* error callback function */
			this.id = __m_Utils.generateGUID();
			//add the query-id to args by default
			this.args.push(this.id);
		}
	
		var DBSQLQuery = function(command){
			this.command = command; /* DBCommand - execution entity of this native api call */
			this.sql =null,
			this.params = null;
			this.successCallback = null;
			this.errorCallback = null;
			this.id = __m_Utils.generateGUID();
		};
	
		/* Main entity which encapsulates all database operations.Used to create 
		 * execute ,queue transactions and commands.Executes commands,transactions
		 * asynchronously preserving the order.
		 * 
		 *  */
		var DBCommand= function(database){
			this.database = database;
			this.successCallback = null;
			this.errorCallback = null;
		
			this.id = __m_Utils.generateGUID();
			this.queryList=[];	/* ordered list of queries */		
			this.ignoreCommit = false; /* mark for commit/rollback */
		};
	
		DBCommand.prototype={	
			/* Validates sql arguments.Throws Error .*/
			validateSql : function(sql,successCallback,errorCallback,params){
				__m_Utils.checkRequired("sql",sql,"string");				
				if(successCallback){
					__m_Utils.checkRequired(successCallback,"function");
				}
				if(errorCallback){
					__m_Utils.checkRequired(errorCallback,"function");
				}
				if(params){
					__m_Utils.checkRequired("params",params, Array);
				}
			},
			/* Just push the queries to the query list Either queues a  command for execution
			 * or calls error callback for query.*/
			executeSql : function(sql,successCallback,errorCallback,params){
				var args = null;
				try{
					this.validateSql(sql,successCallback,errorCallback,params);
				}catch(e){								
					var err= getSqlerror();
					this.invokeErrorCallback(errorCallback,err);
					return;
				}
			
				//create new query
				var query = new DBSQLQuery(this);			
				query.sql = sql;
				query.params = params || [];
				query.successCallback = successCallback;
				query.errorCallback = errorCallback;
			
				//append to query list of transactions
				this.queryList.push(query);
			},
			/* Attempts to submit queued query/command to native bridge
			*  or throws  Error.
			*/
			executeQuery: function(query){	
			
				if(!query || (!(query instanceof DBSQLQuery)
						&& !(query instanceof DBSQLCommand))) {
					throw new TypeError("The argument query should be of type DBSQLQuery or DBSQLCommand");
				}			
			
				if(query instanceof DBSQLQuery){
				
				 	//append to database query queue			
					_pushQuery(query);
				
					var dbId = this.database.id;
					var options = {
						id:dbId,
						queryId:query.id,
						sql:query.sql,
						params:query.params
					};
				
					Bridge.invoke(DatabaseFeatureName,"executeQuery",[options],_onQueryComplete);
				}
				//This snippet currently not in use.Will be useful if we add helper functions
				if(query instanceof DBSQLCommand){				
					var command = query;
				
					_pushQuery(command);
				
					if(command && command.name) {
						Bridge.invoke(DatabaseFeatureName,command.name,command.args,_onQueryComplete);
					}
					else {
						throw new Error("Invalid command");				
					}
				}
			},
		
			begin: function(f_callback){			
				var o = { context: this,callback: f_callback };
			
				Bridge.invoke(DatabaseFeatureName,"beginTransaction",[this.database.id],o);			
			},
		
			/*
			 * Async commit or throws Exception 
			 */ 
			commit: function(f_callback){			
				var o = { context: this,callback: f_callback };
			
				Bridge.invoke(DatabaseFeatureName,"commit",[this.database.id],o);			
			},		
			rollback:function(f_callback){
				var o = { context: this,callback: f_callback };					
			
				Bridge.invoke(DatabaseFeatureName,"rollback",[this.database.id],o);			
			},
			/* Executes next query if exists or triggers completion of transaction .
			*  Does not throw any exception.
			*/
			executeNextQuery : function(query){
				if(query){
					//delete current query from database queue
					_removeQuery(query);
				}
			
				if(this.queryList.length > 0){
					var query = this.queryList.shift();
				
					//TODO: handle invalid queries
					if(query){ 
						try{
							//check if it's native command or query call
							this.executeQuery(query);						
						}catch(err){
							err= getSqlerror(err);
							this.invokeErrorCallback(query.errorCallback,err);
							this.executeNextQuery(query);
						}
					}
					else{
						//not a valid query proceed with next ignore:it never happens
						this.executeNextQuery();
					}
				}
				else{
					this.completeTransaction();
				}
			},
			//invoke success callback if callback has error omit further processing of 
			// transaction and rollback
			queryComplete:function(query,result){
			
				try {
					var resultSet = null;
					if(query instanceof DBSQLQuery){
						 var data = result.data;
						 resultSet = new DBResultset();
						 resultSet.insertId = data.insertId;
						 resultSet.rowsAffected = data.rowsAffected;
						 if(__m_Utils.isArray(data.rows)) {
							resultSet.rows = data.rows;
						 }
					 
						 try {
								//invoke query success callback
								if(query.successCallback && typeof query.successCallback === "function"){	
									if(this.ignoreCommit){
										query.successCallback(resultSet);
									}
									else{
										query.successCallback(this,resultSet);
									}
								}							
						 } catch(err){
							 if(!this.ignoreCommit){
								 this.processError(err);
							 }
							 else{
								 this.executeNextQuery(query);
							 }
							 return;
						 }
					}
					if(query instanceof DBSQLCommand){
							try {
								//invoke query success callback
								if(query.successCallback && typeof query.successCallback === "function"){	
									var data = result.data;
									if(this.ignoreCommit){
										query.successCallback(data);
									}
									else{
										query.successCallback(this,data);
									}
								}							
						 } catch(err){						 
							 if(!this.ignoreCommit){
								 this.processError(err);
							 }
							 else{
								 this.executeNextQuery(query);
							 }
							 return;
						 }
					}
					this.executeNextQuery(query);
				} catch(ex){				
					//need to process this as query failure
					var e = getSqlerror(ex);
					this.queryFailed(query,e);
				}
			},
			/*
			 * Process query failure.Execute error callback ,if fails terminate transaction(see processError method)
			 * .If error callback execution successful trigger next transaction safely.
			*/
			queryFailed : function(query,error){
				try {				
				
					try { //Note: query can be instance of DBSQLQuery or DBSQLCommand
						if(query.errorCallback && typeof query.errorCallback === "function"){
							query.errorCallback(error);
						}
					} catch(err){
						if(!this.ignoreCommit){
							 this.processError(err);
						}
						else{
							 _executeNextCommand();
						 }
						return;
					}				
					this.executeNextQuery(query);				
				} catch(err){			
				}				
			},
			/*
			 * Marks safe completion of command.Is responsible for triggering next queued command.
			*/
			completeTransaction: function(){
			
				try {					
					if(this.queryList.length == 0){				
						try{
							//transactions executing as plain sql's ignore commit..
							//usually we do not have success callback for plain commands
							//however we can support if needed.
							if(this.ignoreCommit) {
								invokeSafeCallback(this.successCallback);
								_executeNextCommand();
								return;
							}
												
							this.commit(function(response){							
								if(response.error){							
									var code = response.error.code;
									var msg = response.error.message;
									var error = new DBError(code,msg);
								
									this.rollback(function(rollbackResponse){	
										try{
											//Get the last error
											if(rollbackResponse.error){
												code = rollbackResponse.error.code;
												msg = rollbackResponse.error.message;
												error = new DBError(code,msg);
											}
										}catch(err){
											error = err;
											error = getSqlError(err);
										}
										this.invokeErrorCallback(this.errorCallback,error);
										_executeNextCommand();
									});						
								}
								else{							
									//successfull commit 
									this.invokeSuccessCallback(this.successCallback);
									_executeNextCommand();								
								}					
							});
						} catch(ex){
							//commit failed 
							throw ex;
						}						
					}
				} catch(err){
					err = getSqlError(err);
					this.invokeErrorCallback(this.errorCallback,err);
					_executeNextCommand();
				}			
			},		
			processError : function(error){
				try{
				
					//clear database query queue
					this.queryList.forEach(function(query,index,array){
						_removeQuery(query);
					});
				
					//clear the pending querylist
					this.queryList=[];
				
					var tmpError = error;
				
					//rollback 
					this.rollback(function(response){
					
						//trigger errorCallback with last sql error
						if(response.error){
							var code = response.code;
							var msg = response.error.message;
							var err= new DBError(code,msg);
							tmpError = err;
						}
					
						this.invokeErrorCallback(this.errorCallback,tmpError);
					
						_executeNextCommand();
					});				
				}catch(err){
					this.queryList = [];
					var e = getSqlerror(err);
					this.invokeErrorCallback(this.errorCallback,e);
					_executeNextCommand();
				}			
			},
			/* Triggers execution of queries in transaction.Usually called
			 * internaly after beginTransaction or in _executeCommand,_executeNextCommand
			 * only when command queryList size > 0.
			 * Throws Exception.		 
			*/
			process : function(){
				if(this.queryList.length > 0){				
					this.executeNextQuery();
				}
				else{ //ignore will never happen
					//prevent invoking commit even if queries are empty				
					this.invokeSuccessCallback(this.successCallback);
				}
			},
			invokeSuccessCallback : function(successCallback,result){
				if(!successCallback) return;
			 
				if(this.ignoreCommit){
					invokeSafeCallback(successCallback,[result]);
				}
				else{
					invokeSafeCallback(successCallback,[this,result]); //this refer to dbtransaction object
				}
			},
			invokeErrorCallback : function(errorCallback,error){			
				if(!errorCallback) return;
				invokeSafeCallback(errorCallback,[error]);				
			},
			/* Adds the query to the command and enqueues the command for execution */ 
			enqueue : function(queries){
				if(queries instanceof Array){
					var queryList = this.queryList;
					queries.forEach(function(query){
						if((query instanceof DBSQLCommand) ||
								(query instanceof DBSQLQuery)){						
							queryList.push(query);
						}
					});								
				}
			}
		};
	
		//Database transaction object to be exposed to user
		function DBTransaction(dbCommand){
			/* Executes sql */
			this.executeSql=function(sql,successCallback,errorCallback,params){
				dbCommand.executeSql(sql,successCallback,errorCallback,params);			
			};
		}
	
		// Database
		var Database = function(){
			this.id = -1;	/* id of this database connection */
		};
	
		Database.prototype.transaction=function(process,successCallback,errorCallback){
			var command = new DBCommand(this);
			command.successCallback = successCallback;
			command.errorCallback = errorCallback;
			if(process){ 
				try {
					var txn = new DBTransaction(command);
				
					//execute transaction callback
					process(txn);				
					_executeCommand(command);
				} catch(err) {
					/* there should be only javascript errors.Transaction shouldn't begin by now. */
					err = getSqlerror(err);
					invokeSafeCallback(errorCallback, [err]);
				}
			}
		};
	
		/* Executes plain sql in the current database connection */
		Database.prototype.executeSql=function(sql,successCallback,errorCallback,params){
			var command = new DBCommand(this);
			try {
				command.ignoreCommit = true; 
			
				//validate errors 
				command.validateSql(sql,successCallback,errorCallback,params);
			
				//create new query
				var query = new DBSQLQuery(command);			
				query.sql = sql;
				query.params = params || [];
				query.successCallback = successCallback;
				query.errorCallback = errorCallback;
			
				command.enqueue([query]);
				_executeCommand(command);
			
			} catch(err) {
				/* there should be only javascript errors.Transaction shouldn't begin by now. */
				err = getSqlerror(err);
				invokeSafeCallback(errorCallback, [err]);
			}		
		};
	
		var invokeSafeCallback = function(fCallback,args,context){
			if(__m_Utils.isOfType(fCallback,"function")){
				try{
					if(context){
						fCallback.apply(context,args);	
					}
					else{
						fCallback.apply(window,args);
					}
				}catch(e){}
			}
		};
	
	
		/*
		 * Receives native response and invokes success or failure processing for a command in the queue.
		 * This method should not have any errors as per js-native contract.
		 */
		var _onQueryComplete = function(response){
			if(response){	
				var id = response.result.queryId;			
				var query = queryQueue[id];
			
				if(query){
					var command = query.command;				
					if(command){
						if(response.error){
							var code = response.code;
							var msg = response.error.message;
							var error = new DBError(code, msg);						
							command.queryFailed(query,error);
						}
						else{
							command.queryComplete(query,response.result);
						}				
					}			
				}
			}
		};	
		var _executeCommand = function(command){		
			if(command){
				if(commandQueue.length > 0) {	
					commandQueue.push(command);
					//TODO: check for duplicate id
				}
				else {
					if(command.queryList.length > 0) {
						commandQueue.push(command);
					
						try{						
							if(command.ignoreCommit) {
								command.process();
							}
							else{					
								//begin transaction
								command.begin(function(response){
									if(response.error)	{
										var code = response.code;
										var msg = response.error.message;
										var e = new DBError(code,msg);
										invokeSafeCallback(command.errorCallback,[e]);		
										_executeNextCommand();
									}
									else{
										command.process();		
									}
								});
							}					
						}catch(err){
							//transaction already pushed just before
							var index = -1;
							commandQueue.forEach(function(tr,i,array){
								if(tr.id === command.id ) {
									index = i;				
								}	
							});
							if(index > -1){
								commandQueue.splice(index,1);
							}
							var e = getSqlerror(err);
							invokeSafeCallback(command.errorCallback,[e]);
							_executeNextCommand();
						}
					}
					else{					
						invokeSafeCallback(command.successCallback);
						_executeNextCommand();
					}
				}
			}
		};
		/*
		 * Triggers next transaction execution safely.This method should not have exception.
		*/
		var _executeNextCommand = function(){
		
			//remove current transaction..
			commandQueue.shift(); 
		
			var command = null;
		
			//should have other queued transactions than the current one..
			if(commandQueue.length > 0){
				try {
					command = commandQueue[0];
				
					if(command && command.queryList.length > 0 ){
						if(command.ignoreCommit) {
							command.process();
						}
						else{					
							//begin transaction
							command.begin(function(response){
								if(response.error)	{
									var code = response.code;
									var msg = response.error.message;
									var e = new DBError(code,msg);
									invokeSafeCallback(command.errorCallback,[e]);
									_executeNextCommand();
								}
								else{
									command.process();		
								}
							});
						}
					}
					else {	
						if(command){
							invokeSafeCallback(command.successCallback);
						}
						_executeNextCommand();
					}
				} catch(err){
					//should not happen..ignore
					if(command){
						var e = getSqlerror(err);
						invokeSafeCallback(command.errorCallback,[e]);
					}
					_executeNextCommand();
				}
			}		
		};
				
		var DB_APP_LEVEL = 1;
		var DB_STORAGE_LEVEL = 2;
		var DB_DOCUMENT_LEVEL = 3;
		
		var _openDatabase = function(name,fCallback,version,password,storageLevel){		
			var level = this.DefaultLevel;
						
			if(arguments.length<2){
				throw new TypeError("The argument name and callback are required");
			}
			if(arguments.length===2){
				__m_Utils.checkRequired("name",name,"string");
				__m_Utils.checkRequired("callback",fCallback,"function");			
			}
			if(arguments.length===3){
				__m_Utils.checkRequired("name",name,"string");
				__m_Utils.checkRequired("callback",fCallback,"function");
				__m_Utils.checkRequired("version",version,"number");				
			}
			if(arguments.length===4){
				__m_Utils.checkRequired("name",name,"string");
				__m_Utils.checkRequired("callback",fCallback,"function");
				__m_Utils.checkRequired("version",version,"number");
				__m_Utils.checkRequired("password",password,"string");				
			}
			
			if(arguments.length===5){
				__m_Utils.checkRequired("name",name,"string");
				__m_Utils.checkRequired("callback",fCallback,"function");
				__m_Utils.checkRequired("version",version,"number");
				__m_Utils.checkRequired("password",password,"string");				
				__m_Utils.checkRequired("level",storageLevel,"number");		
				
				if(storageLevel <= DB_DOCUMENT_LEVEL
					&& storageLevel >= this.DefaultLevel) {
						level = storageLevel;
				}				
			}
			version = version || 1;
			
			function responseCallback(response){			
				if(response.error){								
					var code = response.error.code;
					var msg = response.error.message;
					var e = new DBError(code,msg);
					fCallback({ error: e,database:null});
				}
				if(response.result){
					var id = response.result;
					var db =  new Database();
					db.id = id;
					fCallback({ error: null,database:db});
				}
			}
			
			Bridge.invoke(DatabaseFeatureName,"openDatabase",[name,level,version,password],responseCallback);	
		};
	
		function _pushQuery(query){
			if(query && query.id){
				queryQueue[query.id] = query;
			}		
		}
	
		function _removeQuery(query){
			if(query && query.id){
				delete queryQueue[query.id];
			}		
		}	
		return {				
			openDatabase: _openDatabase,
			DefaultLevel : DB_APP_LEVEL,
			APPLICATION_LEVEL : DB_APP_LEVEL,
			STORAGE_LEVEL : DB_STORAGE_LEVEL
		};			
	})();
	
	mowbly.addFeature(DatabaseFeatureName, 'DatabaseShell', DatabaseShell);
	mowbly.exportHelperMethod("openDatabase", "openDatabase", "DatabaseShell");
})(window);