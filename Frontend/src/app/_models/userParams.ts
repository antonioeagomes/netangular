import { User } from "./user";

export class UserParams {
    minAge = 18;
    maxAge = 150;
    pageNumber = 1;
    pageSize = 5;

    constructor (user: User) {
        
    }
}