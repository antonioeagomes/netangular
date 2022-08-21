import { Photo } from "./photo";

export interface Member {
    id: number;
    username: string;
    age: number;
    knownAs: string;
    createAt: Date;
    lastActive: Date;
    introduction: string;
    city: string;
    country: string;
    photoUrl: string;
    photos: Photo[];
}