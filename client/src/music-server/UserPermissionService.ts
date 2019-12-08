import { UserPermissionsRepository } from "./UserPermissionsRepository";

const defaultPermissions = {
    canEditTrackData: false,
    canFetchTrackRecommendations: false
}

export class UserPermissionService {

    constructor(private repo: UserPermissionsRepository){
    }

    async getPermissionsFromKey(key?: string){
        console.log('UserPermissionService.getPermissionsFromKey start')
        if(!key)
            return defaultPermissions
        const p = await this.repo.getPermissionsForKey(key)
        console.log(p)
        console.log('UserPermissionService.getPermissionsFromKey end')
        return p || defaultPermissions
    }
}