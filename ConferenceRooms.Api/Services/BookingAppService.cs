using ConferenceRooms.Api.Domain.Pricing;
using ConferenceRooms.Api.Dtos;
using ConferenceRooms.Api.Exceptions;
using ConferenceRooms.Api.Models;
using ConferenceRooms.Api.Repositories;

namespace ConferenceRooms.Api.Services;

public class BookingAppService : IBookingAppService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IRoomRepository _roomRepo;
    private readonly IPricingService _pricingService;

    public BookingAppService(IBookingRepository bookingRepo, IRoomRepository roomRepo, IPricingService pricingService)
    {
        _bookingRepo = bookingRepo;
        _roomRepo = roomRepo;
        _pricingService = pricingService;
    }

    public async Task<BookingDto?> GetByIdAsync(int id)
    {
        Booking? booking = await _bookingRepo.GetByIdAsync(id);
        if (booking is null) return null;
        PriceBreakdown priceBreakdown = CalculatePriceBreakdown(booking);
        return ToDto(
            booking,
            priceBreakdown
        );
    }

    public async Task<BookingDto> CreateAsync(CreateBookingRequest request)
    {
        DateTime endTime = request.StartTime.Add(request.Duration);
        IReadOnlyList<int> serviceIds = request.ServiceIds ?? [];

        Room room = await _roomRepo.GetByIdAsync(request.RoomId) ?? throw new NotFoundException("Room not found for this booking");
        List<int> notFoundServices = [.. serviceIds.Except(room.Services.Select(service => service.Id))];
        if (notFoundServices.Count > 0) throw new NotFoundException($"Services with id: {string.Join(", ", notFoundServices)} is missing for room");

        List<BookingService> bookingServices = room.Services
            .Where(
                service => serviceIds.Contains(service.Id)
            )
            .Select(
                service => new BookingService
                {
                    RoomServiceId = service.Id,
                    RoomService = service,
                    PriceAtBooking = service.Price
                }
            )
            .ToList();

        bool isOverlaps = await _bookingRepo.HasOverlapAsync(request.RoomId, request.StartTime, endTime);
        if (isOverlaps) throw new OverlapException("Booking has times overlaps");

        Booking booking = new()
        {
            RoomId = request.RoomId,
            Room = room,
            StartTime = request.StartTime,
            EndTime = endTime,
            CreatedAt = DateTime.UtcNow,
            Services = bookingServices
        };

        PriceBreakdown priceBreakdown = CalculatePriceBreakdown(booking);

        booking.TotalPrice = priceBreakdown.Total;

        _bookingRepo.Add(booking);
        await _bookingRepo.SaveChangesAsync();
        return ToDto(booking, priceBreakdown);
    }

    private PriceBreakdown CalculatePriceBreakdown(Booking booking)
    {
        return _pricingService.Calculate(
            booking.Room.BasePricePerHour,
            booking.StartTime,
            booking.EndTime,
            booking.Services.Select(service => service.PriceAtBooking)
        );
    }

    private static BookingDto ToDto(Booking booking, PriceBreakdown priceBreakdown)
    {
        List<BookingServiceDto> services = booking.Services
            .Select(
                service => new BookingServiceDto(
                    service.RoomServiceId,
                    service.RoomService.Name,
                    service.PriceAtBooking
                )
            )
            .ToList();

        return new BookingDto(
            booking.Id,
            booking.RoomId,
            booking.StartTime,
            booking.EndTime,
            services,
            priceBreakdown
        );
    }
}