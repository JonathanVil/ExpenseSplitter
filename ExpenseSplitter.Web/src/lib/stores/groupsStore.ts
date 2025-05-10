import {dataApi, type GroupListItem} from "$lib/services/api";


export async function getGroups(): Promise<GroupListItem[]> {
    const response = await dataApi.listGroups();
    return response.groups;
}

export function createGroup(name : string) : Promise<string> {
    return dataApi.createGroup(name);
}