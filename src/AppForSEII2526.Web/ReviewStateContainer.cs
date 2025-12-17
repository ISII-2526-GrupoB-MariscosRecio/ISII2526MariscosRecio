

using AppForSEII2526.Web.API;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;

namespace AppForSEII2526.Web {

    public class ReviewStateContainer {

        //we create an instance of Rental when an instance of RentalStateContainer is created
        public ReviewForCreateDTO Review { get; private set; } = new ReviewForCreateDTO() {
            ReviewItems = new List<ReviewItemDTO>()
        };

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();



        public void AddDeviceToReview(DeviceParaReseñasDTO device) {
            //before adding a device we checked whether it has been already added
            if (!Review.ReviewItems.Any(ri => ri.DeviceId == device.Id))
                //we add it if it is not in the list
                Review.ReviewItems.Add(new ReviewItemDTO() {
                    DeviceId = device.Id,
                    ModelName = device.Model,
                    DeviceYear = device.Year
                }
            );

        }

        //to delete devices from the list of selected devices
        public void RemoveReviewItemToReview(ReviewItemDTO item) {
            Review.ReviewItems.Remove(item);

        }

        //we eliminate all the devices from the list
        public void ClearReviewingCart() {
            Review.ReviewItems.Clear();

        }

        //we have already finished the process of reviewing, thus, we create a new Review
        public void ReviewProcessed() {
            //we have finished the review process so we create a new object without data
            Review = new ReviewForCreateDTO() {
                ReviewItems = new List<ReviewItemDTO>()
            };
        }
    }
}
