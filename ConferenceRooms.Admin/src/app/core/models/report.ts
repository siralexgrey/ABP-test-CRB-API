export interface RoomRevenue {
  roomId: number;
  roomName: string;
  amount: number;
}

export interface ServiceRevenue {
  roomServiceId: number;
  name: string;
  amount: number;
}

export interface RevenueReport {
  total: number;
  byRoom: RoomRevenue[];
  byService: ServiceRevenue[];
}
