import { Photo } from './photo';

export interface User {
    id: number;
    username: string;
    knownAs: string;
    age: number;
    gender: string;
    createdProfile: Date;
    updatedlastOnline: string;
    updatedcreatedProfile: string;
    lastOnline: Date;
    photoURL: string;
    mainPhoto: string;
    city: string;
    country: string;
    interests?: string;
    introduction?: string;
    lookingFor?: string;
    photos?: Photo[];



}
