import { Db } from "mongodb";
import { UserPermissions } from "../dataModels";
import { MongoBaseRepository } from "../utils";
import { dissoc } from "ramda";

export class UserPermissionsRepository extends MongoBaseRepository<UserPermissions & {forKey: string}> {

    constructor(db: Db){
        super(db, 'userPermissions')
    }

    getPermissionsForKey(key: string){
        return this.collection.findOne({forKey: key}).then(d => dissoc('forKey', d) as UserPermissions)
    }
}