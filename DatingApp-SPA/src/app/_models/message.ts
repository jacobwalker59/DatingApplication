import { NumberValueAccessor } from '@angular/forms';

export interface Message {

    id: number;
    senderId: number;
    senderKnownAs: number;
    senderPhotoURL: string;
    recipientId: number;
    recipientKnownAs: string;
    recipientPhotoURL: string;
    content: string;
    isRead: boolean;
    dateRead: Date;
    messageSent: Date;

}
