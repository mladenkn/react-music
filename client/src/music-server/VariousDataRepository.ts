import { Db, Collection } from "mongodb";
import { assoc } from "ramda";

const insertedAtKey = 'savedAt'

export class VariousDataRepository {

    // private homeSectionStates: Collection<PersistableHomeState>

    // constructor(db: Db){
    //     this.homeSectionStates = db.collection<PersistableHomeState>('homeSectionStates')
    // }
    
    // async saveHomeSectionState(state: PersistableHomeState){
    //     const withDate = assoc(insertedAtKey, new Date(), state)
    //     await this.homeSectionStates.insertOne(withDate)
    // }
    
    // getLastHomeSectionState(){
    //     return this.homeSectionStates.find({}).sort({_id: -1}).limit(1).toArray().then(a => a[0])
    // }
}