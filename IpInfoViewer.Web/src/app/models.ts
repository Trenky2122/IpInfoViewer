export interface IpAdress{
  id: number;
  ipValue: string;
  countryCode: string;
  city: string;
  latitude: number;
  longitude: number;
}

export interface MapIpAddressRepresentation{
  id: number;
  averagePingRtT: number;
  maximumPingRtT: number;
  minimumPingRtT: number;
  ipAddressesCount: number;
  latitude: number;
  longitude: number;
}

export interface StringResponse{
  response: string;
}
