//
//  DigitalClockView.swift
//  ClockLearner
//
//  Created by Yang Wang on 2/5/25.
//

import SwiftUI

struct DigitalClockView: View {
    @ObservedObject var viewModel: ClockViewModel
    
    var body: some View {
        HStack {
            Picker("Hour", selection: $viewModel.hour) {
                ForEach(1...12, id: \ .self) { Text("\($0)") }
            }
            .pickerStyle(WheelPickerStyle())
            
            Text(":")
                .font(.largeTitle)
            
            Picker("Minute", selection: $viewModel.minute) {
                ForEach(0...59, id: \ .self) { Text(String(format: "%02d", $0)) }
            }
            .pickerStyle(WheelPickerStyle())
        }
        .font(.largeTitle)
    }
}
