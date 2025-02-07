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
                ForEach(1...12, id: \ .self) { h in
                    HStack {
                        Text("\(h, specifier: "%02d")")
                    }
                }
            }
            .pickerStyle(WheelPickerStyle())
            .scaleEffect(2)
            .frame(width: 80, height: 60)
            
            Text(":")
                .font(.largeTitle)
                .frame(width: 60, height: 60)
            
            Picker("Minute", selection: $viewModel.minute) {
                ForEach(0...59, id: \ .self) { m in
                    HStack {
                        Text(String(format: "%02d", m))
                    }
                }
            }
            .pickerStyle(WheelPickerStyle())
            .scaleEffect(2)
            .frame(width: 80, height: 60)
        }
        .font(.largeTitle)
    }
}
