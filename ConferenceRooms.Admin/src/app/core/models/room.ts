export interface RoomService {
  id: number;
  name: string;
  price: number;
}

export interface Room {
  id: number;
  name: string;
  capacity: number;
  basePricePerHour: number;
  services: RoomService[];
}

export interface CreateRoomServiceRequest {
  name: string;
  price: number;
}

export interface CreateRoomRequest {
  name: string;
  capacity: number;
  basePricePerHour: number;
  services?: CreateRoomServiceRequest[];
}

export interface UpdateRoomRequest {
  name: string;
  capacity: number;
  basePricePerHour: number;
}
