//
//  ClockViewModel.swift
//  ClockLearner
//
//  Created by Yang Wang on 2/5/25.
//

import SwiftUI

class ClockViewModel: ObservableObject {
    @Published var hour: Int = 3
    @Published var minute: Int = 0
    
    func setTimeFromAnalog(hour: Int, minute: Int) {
        self.hour = hour
        self.minute = minute
    }
    
    func setTimeFromDigital(hour: Int, minute: Int) {
        self.hour = hour
        self.minute = minute
    }
}
